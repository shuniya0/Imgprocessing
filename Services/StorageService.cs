using ImgUploadApi.Models;
using ImgUploadApi.DTO;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace ImgUploadApi.Services
{
    public class StorageService : IStorageService
    {
        //private readonly IConfigurationManager _config;
        private readonly AwsConfig _awsConfig;

        public StorageService(IOptions<AwsConfig> awsConfig)
        {
            _awsConfig = awsConfig.Value;
        }

        public async Task<S3ResponseDto> UploadFileAsync(S3Object obj)
        {
            //var awsCredentialsValues = _config.ReadS3Credentials();

            Console.WriteLine($"Key: {_awsConfig.AWSAccessKey}, Secret: {_awsConfig.AWSSecretKey}");

            var credentials = new BasicAWSCredentials(_awsConfig.AWSAccessKey, _awsConfig.AWSSecretKey);

            var config = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };

            var response = new S3ResponseDto();
            try
            {
                var uploadRequest = new TransferUtilityUploadRequest()
                {
                    InputStream = obj.InputStream,
                    Key = obj.Name,
                    BucketName = obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };

                // initialise client
                using var client = new AmazonS3Client(credentials, config);

                // initialise the transfer/upload tools
                var transferUtility = new TransferUtility(client);

                // initiate the file upload
                await transferUtility.UploadAsync(uploadRequest);

                response.StatusCode = 201;
                response.Message = $"{obj.Name} has been uploaded sucessfully";
            }
            catch (AmazonS3Exception s3Ex)
            {
                response.StatusCode = (int)s3Ex.StatusCode;
                response.Message = s3Ex.Message;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
