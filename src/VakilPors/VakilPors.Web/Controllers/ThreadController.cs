using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Shared.Response;
using VakilPors.Web.Controllers;

namespace VakilPors.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ThreadController : MyControllerBase
    {
        private readonly IThreadService _threadService;
        private readonly ILogger<ThreadController> _logger;
        public ThreadController(IThreadService threadService, ILogger<ThreadController> logger)
        {
            _threadService = threadService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(ThreadDto threadDto)
        {
            _logger.LogInformation($"create new thread {threadDto.Title}");
            var result = await _threadService.CreateThread(getUserId(), threadDto);
            return Ok(new AppResponse<object>(result, "thread created"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateThread(ThreadDto threadDto)
        {
            _logger.LogInformation($"update thread {threadDto.Id}");
            var result = await _threadService.UpdateThread(getUserId(), threadDto);
            return Ok(new AppResponse<object>(result, "thread updated"));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteThread(ThreadDto threadDto)
        {
            _logger.LogInformation($"delete thread {threadDto.Id}");
            var result = await _threadService.DeleteThread(getUserId(), threadDto.Id);
            return Ok(new AppResponse<object>(result, "thread deleted"));
        }

        [HttpGet]
        public async Task<IActionResult> GetThreadList()
        {
            var result = await _threadService.GetThreadList();
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetThreadWithComments(int threadId)
        {
            var result = await _threadService.GetThreadWithComments(threadId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> LikeThread(int threadId)
        {
            var result = await _threadService.LikeThread(threadId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> UndoLikeThread(int threadId)
        {
            var result = await _threadService.UndoLikeThread(threadId);
            return Ok(new AppResponse<object>(result, "success"));
        }

    }
}
