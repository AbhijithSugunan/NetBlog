using System;
using System.Threading.Tasks;
using AutoMapper;
using Blog.API.Contracts.V1.Responses;
using Blog.API.Data;
using Blog.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.Services
{
    public class CommentService: ICommentService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public CommentService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<bool> CreateCommentAsync(Comment comment)
        {
            await _dataContext.Comments.AddAsync(comment);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId)
        {
            var comment = await _dataContext.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
                return false;
            _dataContext.Comments.Remove(comment);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }

        public async Task<CommentResponse> UpdateCommentAsync(Comment comment)
        {
            comment.UpdatedAt = DateTime.UtcNow;
            _dataContext.Comments.Update(comment);
            var updated = await _dataContext.SaveChangesAsync();
            return updated > 0 ? _mapper.Map<CommentResponse>(comment) : null;
        }

        public async Task<Comment> GetCommentByIdAsync(Guid commentId)
        {
            return await _dataContext.Comments.FirstOrDefaultAsync(comment => comment.Id == commentId);
        }

        public async Task<bool> UserOwnsComment(Guid commentId, string userId)
        {
            var comment = await _dataContext.Comments.AsNoTracking().SingleOrDefaultAsync(x => x.Id.Equals(commentId));
            if (comment == null)
            {
                return false;
            }

            return comment.CreatorId == userId;
        }
    }
}
