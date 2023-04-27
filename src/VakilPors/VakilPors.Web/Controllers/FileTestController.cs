﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;

namespace VakilPors.Api.Controllers
{
    
    [ApiController]
    [Route("[controller]/[action]")]
    public class FileTestController : Controller
    {
        private readonly IAwsFileService _fileService;

        public FileTestController(IAwsFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            return Ok(await _fileService.UploadAsync(file));
        }

        [HttpGet]
        public IActionResult GetUrl(string key)
        {
            return Ok(_fileService.GetFileUrl(key));
        }
    }
}
