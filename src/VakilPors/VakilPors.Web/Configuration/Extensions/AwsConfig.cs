using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Internal;
using Amazon.Runtime;
using Amazon.S3;

namespace VakilPors.Api.Configuration.Extensions
{
    public static class AwsConfig
    {
        public static void AddAwsFileService(this IServiceCollection services, IConfiguration configuration)
        {

            string fileEndPoint = Environment.GetEnvironmentVariable("FILE_API_ENDPOINT") ?? Environment.GetEnvironmentVariable("FILE_API_ENDPOINT", EnvironmentVariableTarget.User) ?? configuration["AWS:ApiEndpoint"];
            string accessKey = Environment.GetEnvironmentVariable("FILE_ACCESS_KEY") ?? Environment.GetEnvironmentVariable("FILE_ACCESS_KEY", EnvironmentVariableTarget.User) ?? configuration["AWS:AccessKey"];
            string secretKey = Environment.GetEnvironmentVariable("FILE_SECRET_KEY") ?? Environment.GetEnvironmentVariable("FILE_SECRET_KEY", EnvironmentVariableTarget.User) ?? configuration["AWS:SecretKey"];

            var awsConfig = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(accessKey, secretKey),
                Region = RegionEndpoint.USEast1,
                DefaultClientConfig = { ServiceURL = fileEndPoint }
            };

            services.AddDefaultAWSOptions(awsConfig);
            services.AddAWSService<IAmazonS3>();
        }
    }
}
