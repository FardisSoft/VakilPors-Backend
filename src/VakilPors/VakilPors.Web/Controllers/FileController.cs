using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;

namespace VakilPors.Api.Controllers
{
    
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileController : Controller
    {
        private readonly IAwsFileService _fileService;

        public FileController(IAwsFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok(await _fileService.UploadAsync(file));
        }
        [HttpPost]
        public async Task<IActionResult> UploadFileMessage(IFormFile file)
        {
            return Ok(await _fileService.UploadFileMessageAsync(file));
        }

        [HttpGet]
        public IActionResult GetUrl(string key)
        {
            return Ok(_fileService.GetFileUrl(key));
        }

        [HttpGet]
        public async Task<IActionResult> Download(string key)
        {
            return Ok(await _fileService.DownloadAsync(key));
        }
        [HttpGet]
        public Task<IActionResult> DownloadSlide()
        {
            return Task.FromResult<IActionResult>(Ok("https://docs.google.com/presentation/d/1ykQvtlZxj7leac1wjd7yQa1JoPqwYXUzG2fD5bcmUpU/edit?usp=sharing"));
        }
    }
}
