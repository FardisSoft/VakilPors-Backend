using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Lawyer;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Core.Domain.Entities;
using VakilPors.Core.Mapper;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ThreadController : MyControllerBase
    {
        private readonly IThreadService _threadService;
        private readonly ILogger<ThreadController> _logger;
        private readonly IMapper _mapper;
        public ThreadController(IThreadService threadService, ILogger<ThreadController> logger)
        {
            _threadService = threadService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateThread(ThreadDto threadDto)
        {
            _logger.LogInformation($"create new thread {threadDto.Title}");
            var result = await _threadService.CreateThread(GetUserId(), threadDto);
            return Ok(new AppResponse<object>(result, "thread created"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateThread(ThreadDto threadDto)
        {
            _logger.LogInformation($"update thread {threadDto.Id}");
            var result = await _threadService.UpdateThread(GetUserId(), threadDto);
            return Ok(new AppResponse<object>(result, "thread updated"));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteThread(int threadId)
        {
            _logger.LogInformation($"delete thread {threadId}");
            var result = await _threadService.DeleteThread(GetUserId(), threadId);
            return Ok(new AppResponse<object>(result, "thread deleted"));
        }

        [HttpGet]
        public async Task<IActionResult> GetThreadList()
        {
            var result = await _threadService.GetThreadList(GetUserId());
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetThreadWithComments([FromQuery] int threadId,[FromQuery] PagedParams pagedParams)
        {
            var result = await _threadService.GetThreadWithComments(GetUserId(), threadId,pagedParams);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> LikeThread(int threadId)
        {
            var result = await _threadService.LikeThread(GetUserId(), threadId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> UndoLikeThread(int threadId)
        {
            var result = await _threadService.UndoLikeThread(GetUserId(), threadId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> SearchThread([FromQuery] PagedParams pagedParams, [FromQuery] SortParams sortParams, [FromQuery] string? Title)
        {
            _logger.LogInformation($"searching for a thread by title {Title}");
            var result = await _threadService.SearchThread(Title,pagedParams,sortParams, GetUserId());  
            return Ok(new AppResponse<object>(result, "success"));
            
        }

    }
}
