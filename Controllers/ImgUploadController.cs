using ImgUploadApi.Services;
using ImgUploadApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImgUploadApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImgUploadController : ControllerBase
    {


        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;

        private readonly ILogger<ImgUploadController> _logger;

        public ImgUploadController(ILogger<ImgUploadController> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        [HttpPost(Name = "UploadFile")]

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // Process file
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileExt = Path.GetExtension(file.FileName);
            var docName = $"{Guid.NewGuid}.{file.FileName}.{fileExt}";
            // call server

            var s3Obj = new S3Object()
            {
                BucketName = "imgprocessintake",
                InputStream = memoryStream,
                Name = docName
            };

            var result = await _storageService.UploadFileAsync(s3Obj);
            // 
            return Ok(result);

        }
    }
}