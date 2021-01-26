using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blog.API.Contracts.V1.Requests;
using Blog.API.Contracts.V1.Responses;
using Blog.API.Domain;
using Blog.API.Domain.Dtos;

namespace Blog.API.Profiles
{
    public class PostProfile: Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostDTO>();
            CreateMap<Comment, CommentResponse>();
            CreateMap<CreateCommentRequest, Comment>();
            
        }
    }
}
