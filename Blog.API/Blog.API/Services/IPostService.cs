using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.API.Domain;
using Blog.API.Domain.Dtos;

namespace Blog.API.Services
{
    public interface IPostService
    {
        Task<bool> CreatePostAsync(Post postToCreate);

        Task<IEnumerable<PostDTO>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(Guid postId);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);

        Task<bool> UserOwnsPost(Guid postId, string userId);


    }
}
