using Amazon.S3;
using Castle.Core.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Services;
using Microsoft.Extensions.Configuration;



namespace VakilPors.Test.Core.Services
{
    public class AwsFileServiceTests
    {
        private readonly AwsFileService awsFileService;
        private readonly Mock<IAwsFileService> _awsFileServiceMock;
        private readonly Mock<IAmazonS3> _s3Client;
        private readonly Mock<Microsoft.Extensions.Configuration.IConfiguration> _configurationMock;
        private readonly string _bucketName;

        public AwsFileServiceTests()
        {
            _s3Client = new Mock<IAmazonS3>();
            _bucketName = "test-bucket";
            _awsFileServiceMock = new Mock<IAwsFileService>();
            _configurationMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();

            awsFileService = new AwsFileService
                (
                _s3Client.Object, _configurationMock.Object);
             
                                      

            
        }
    }
}
