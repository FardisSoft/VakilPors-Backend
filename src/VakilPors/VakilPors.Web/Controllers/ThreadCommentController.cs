using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VakilPors.Core.Contracts.Services;
using VakilPors.Core.Domain.Dtos;
using VakilPors.Core.Domain.Dtos.Params;
using VakilPors.Shared.Response;

namespace VakilPors.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class ThreadCommentController : MyControllerBase
    {
        private readonly IThreadCommentService _threadCommentService;
        private readonly ILogger<ThreadCommentController> _logger;

        public ThreadCommentController(ILogger<ThreadCommentController> logger,
            IThreadCommentService threadCommnetService)
        {
            _logger = logger;
            _threadCommentService = threadCommnetService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(ThreadCommentDto commentDto)
        {
            _logger.LogInformation($"create new comment for thread : {commentDto.ThreadId}");
            var result = await _threadCommentService.CreateComment(GetUserId(), commentDto);
            return Ok(new AppResponse<object>(result, "comment created"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateComment(ThreadCommentDto commentDto)
        {
            _logger.LogInformation($"update comment {commentDto.Id}");
            var result = await _threadCommentService.UpdateComment(GetUserId(), commentDto);
            return Ok(new AppResponse<object>(result, "comment updated"));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            _logger.LogInformation($"delete comment {commentId}");
            var result = await _threadCommentService.DeleteComment(GetUserId(), commentId);
            return Ok(new AppResponse<object>(result, "comment deleted"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsForThread([FromQuery] int threadId,
            [FromQuery] PagedParams pagedParams)
        {
            var result = await _threadCommentService.GetCommentsForThread(GetUserId(), threadId, pagedParams);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentById(int commentId)
        {
            var result = await _threadCommentService.GetCommentById(GetUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            var result = await _threadCommentService.LikeComment(GetUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> UndoLikeComment(int commentId)
        {
            var result = await _threadCommentService.UndoLikeComment(GetUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> SetAsAnswer(int commentId)
        {
            var result = await _threadCommentService.SetAsAnswer(GetUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> UndoSetAsAnswer(int commentId)
        {
            var result = await _threadCommentService.UndoSetAsAnswer(GetUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }
    }
}