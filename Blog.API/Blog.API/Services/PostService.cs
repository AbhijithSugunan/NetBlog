using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.API.Data;
using Blog.API.Domain;
using Blog.API.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.Services
{
    public class PostService : IPostService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public PostService(DataContext context, IMapper mapper)
        {
            _dataContext = context;
            _mapper = mapper;
        }
        
        public async Task<bool> CreatePostAsync(Post postToCreate)
        {
            await _dataContext.Posts.AddAsync(postToCreate);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<IEnumerable<PostDTO>> GetPostsAsync()
        {
            var posts = _dataContext.Posts.Select(x => new PostDTO
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                Published = x.Published,
                Title = x.Title,
                UpdatedAt = x.UpdatedAt,
                PublishedAt = x.PublishedAt,
                UserId = x.UserId,
                Comments =  _dataContext.Comments.Where(c => c.PostId == x.Id).ToList()
                
            });
            var postToReturn = _mapper.Map<IEnumerable<PostDTO>>(posts);
            return postToReturn;
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await _dataContext.Posts.FirstOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            _dataContext.Posts.Update(postToUpdate);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await _dataContext.Posts.FirstOrDefaultAsync(post => post.Id == postId);
            if (post == null)
                return false;
            _dataContext.Posts.Remove(post);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<bool> UserOwnsPost(Guid postId, string userId)
        {
            var post = _dataContext.Posts.AsNoTracking().SingleOrDefault(x => x.Id == postId);
            if (post == null)
            {
                return false;
            }

            return post.UserId == userId;
        }
        
        
    }
}
