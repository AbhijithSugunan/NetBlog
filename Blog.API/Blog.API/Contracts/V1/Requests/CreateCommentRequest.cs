using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.API.Contracts.V1.Requests
{
    public class CreateCommentRequest
    {
        public string PostId { get; set; }
        public string Content { get; set; }
    }
}
