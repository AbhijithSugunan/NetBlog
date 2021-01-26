using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Blog.API.Contracts;
using Blog.API.Contracts.V1.Requests;
using Blog.API.Contracts.V1.Responses;
using Blog.API.Domain;
using Blog.API.Extensions;
using Blog.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers.V1
{
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService, IMapper mapper)
        {
            _commentService = commentService;
            _mapper = mapper;
        }

        [HttpPost(ApiRoutes.Comments.Create)]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
        {
            var mappedComment = _mapper.Map<Comment>(request);
            mappedComment.CreatorId = HttpContext.GetUseId();

            var creationStatus = await _commentService.CreateCommentAsync(mappedComment);
            if (!creationStatus)
                return StatusCode((int)HttpStatusCode.InternalServerError);
            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Comments.Get.Replace("{postId}", mappedComment.Id.ToString());

            var commentResponse = new CreateCommentResponse() { Id = mappedComment.Id };

            return Created(locationUri, commentResponse);

        }

        [HttpPut(ApiRoutes.Comments.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid commentId, [FromBody] UpdateCommentRequest postRequest)
        {
            var userOwnsPost = await _commentService.UserOwnsComment(commentId, HttpContext.GetUseId());
            if (!userOwnsPost)
            {
                return BadRequest(error: new { Error = "You do not owns this comment" });
            }

            var comment = await _commentService.GetCommentByIdAsync(commentId);
            comment.Content = postRequest.Content;
            comment.CreatorId = HttpContext.GetUseId();
            
            var updated = await _commentService.UpdateCommentAsync(comment);
            if (updated != null) 
                return Ok(comment);
            return NotFound();
        }

        [HttpDelete(ApiRoutes.Comments.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid commentId)
        {
            var userOwnsPost = await _commentService.UserOwnsComment(commentId, HttpContext.GetUseId());
            if (!userOwnsPost)
            {
                return BadRequest(error: new { Error = "You do not owns this comment" });
            }

            var deletedStatus = await _commentService.DeleteCommentAsync(commentId);
            if (deletedStatus)
                return NoContent();
            return NotFound();
        }

    }
}
