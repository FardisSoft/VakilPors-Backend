using Amazon.S3;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.S3.Internal;

namespace VakilPors.Test.Core.Services
{
    public class AwsFileServiceTests
    {
        private readonly AwsFileService awsFileService;
        private readonly Mock<IAwsFileService> _awsFileServiceMock;
        private readonly Mock<IAmazonS3> _s3Client;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly string _bucketName;

        public AwsFileServiceTests()
        {
            _s3Client = new Mock<IAmazonS3>();
            _bucketName = "vakilpors-disk";
            _awsFileServiceMock = new Mock<IAwsFileService>();
            _configurationMock = new Mock<IConfiguration>();

            awsFileService = new AwsFileService
                (
                _s3Client.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task UploadAsync()
        {
            //Arrange 
            
            var key = Guid.NewGuid().ToString();
            var request = new PutObjectRequest { Key = key  , BucketName = _bucketName , InputStream = null };
            var response = new PutObjectResponse { ETag = "sample" };
            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.Length).Returns(1024);
            formFileMock.Setup(f => f.OpenReadStream());
            _s3Client.Setup(u => u.PutObjectAsync(It.IsAny<PutObjectRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);


            // Act
            var result = await awsFileService.UploadAsync(formFileMock.Object);

            //Assert
            Assert.NotNull(result);
            Assert.StartsWith("https://api.fardissoft.ir/File/Download?key=", result);
        }


    }
}
