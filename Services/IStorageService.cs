using ImgUploadApi.Models;
using ImgUploadApi.DTO;

namespace ImgUploadApi.Services
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAsync(S3Object obj);
    }
}
