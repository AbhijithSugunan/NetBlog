using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Contracts.V1.Responses
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid CreatorId { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
