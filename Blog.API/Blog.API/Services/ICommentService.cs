using System;
using System.Threading.Tasks;
using Blog.API.Contracts.V1.Responses;
using Blog.API.Domain;

namespace Blog.API.Services
{
    public interface ICommentService
    {
        Task<bool> CreateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Guid commentId);
        Task<CommentResponse> UpdateCommentAsync(Comment comment);
        Task<Comment> GetCommentByIdAsync(Guid commentId);
        Task<bool> UserOwnsComment(Guid commentId, string userId);
    }
}
