using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using VakilPors.Shared.Services;

namespace VakilPors.Core.Contracts.Services
{
    public interface IAwsFileService : IScopedDependency    
    {
        Task<string> UploadAsync(IFormFile file);

        Task<Stream> DownloadAsync(string key);

        string GetFileUrl(string key);
    }
}
