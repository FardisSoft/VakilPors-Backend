﻿using Microsoft.AspNetCore.Authorization;
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
    public class ThreadCommentController : MyControllerBase
    {
        private readonly IThreadCommentService _threadCommentService;
        private readonly ILogger<ThreadCommentController> _logger;
        public ThreadCommentController(ILogger<ThreadCommentController> logger, IThreadCommentService threadCommnetService)
        {
            _logger = logger;
            _threadCommentService = threadCommnetService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment(ThreadCommentDto commentDto)
        {
            _logger.LogInformation($"create new comment for thread : {commentDto.ThreadId}");
            var result = await _threadCommentService.CreateComment(getUserId(), commentDto);
            return Ok(new AppResponse<object>(result, "comment created"));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateComment(ThreadCommentDto commentDto)
        {
            _logger.LogInformation($"update comment {commentDto.Id}");
            var result = await _threadCommentService.UpdateComment(getUserId(), commentDto);
            return Ok(new AppResponse<object>(result, "comment updated"));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            _logger.LogInformation($"delete comment {commentId}");
            var result = await _threadCommentService.DeleteComment(getUserId(), commentId);
            return Ok(new AppResponse<object>(result, "comment deleted"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentsForThread(int threadId)
        {
            var result = await _threadCommentService.GetCommentsForThread(getUserId(), threadId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCommentById(int commentId)
        {
            var result = await _threadCommentService.GetCommentById(getUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> LikeComment(int commentId)
        {
            var result = await _threadCommentService.LikeComment(getUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> UndoLikeComment(int commentId)
        {
            var result = await _threadCommentService.UndoLikeComment(getUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> SetAsAnswer(int commentId)
        {
            var result = await _threadCommentService.SetAsAnswer(getUserId() , commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

        [HttpGet]
        public async Task<IActionResult> UndoSetAsAnswer(int commentId)
        {
            var result = await _threadCommentService.UndoSetAsAnswer(getUserId(), commentId);
            return Ok(new AppResponse<object>(result, "success"));
        }

    }
}
