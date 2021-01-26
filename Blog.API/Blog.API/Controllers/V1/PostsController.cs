using System;
using System.Threading.Tasks;
using Blog.API.Contracts;
using Blog.API.Contracts.V1.Requests;
using Blog.API.Contracts.V1.Responses;
using Blog.API.Domain;
using Blog.API.Extensions;
using Blog.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _postService.GetPostsAsync());
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        [HttpPut(ApiRoutes.Posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest postRequest)
        {

            var userOwnsPost = await _postService.UserOwnsPost(postId, HttpContext.GetUseId());
            if (!userOwnsPost)
            {
                return BadRequest(error: new { Error = "You do not owns this post" });
            }

            var post = await _postService.GetPostByIdAsync(postId);
            post.Content = postRequest.Content;
            post.Title = postRequest.Title;
            var updated = await _postService.UpdatePostAsync(post);
            if (updated)
                return Ok(post);
            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Create)]
        public async Task<IActionResult> Create()
        {
            var post = new Post()
            {
                UserId = HttpContext.GetUseId()
            };

            await _postService.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var postResponse = new CreatePostResponse { Id = post.Id };

            return Created(locationUri, postResponse);
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {

            var userOwnsPost = await _postService.UserOwnsPost(postId, HttpContext.GetUseId());
            if (!userOwnsPost)
            {
                return BadRequest(error: new { Error = "You do not owns this post" });
            }

            var deleted = await _postService.DeletePostAsync(postId);
            if (deleted)
                return NoContent();
            return NotFound();
        }
        
    }
}
