using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using VakilPors.Core.Contracts.Services;

namespace VakilPors.Core.Services
{
    public class AwsFileService : IAwsFileService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public AwsFileService(IAmazonS3 amazonS3, IConfiguration configuration)
        {
            _s3Client = amazonS3;
            _bucketName = Environment.GetEnvironmentVariable("FILE_BUCKET") ?? configuration["AWS:BucketName"];
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var url = "https://api.fardissoft.ir/File/Download?key=";

            if (file == null || file.Length < 1)
                return null;

            var key = Guid.NewGuid().ToString();
            var stream = file.OpenReadStream();
            
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream
            };

            var response = await _s3Client.PutObjectAsync(request);

            if (response.ETag == null)
                return null;

            return url+key;
        }
        public async Task<Stream> DownloadAsync(string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request);

            if (response.ResponseStream == null)
                return null;

            return response.ResponseStream;
        }
        public string GetFileUrl(string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Expires = DateTime.Now.AddDays(1)
            };
            
            return _s3Client.GetPreSignedURL(request);
        }
    }
}
