using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Blog.API.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUseId(this HttpContext httpContext)
        {
            return httpContext.User == null ? string.Empty : httpContext.User.Claims.Single(x => x.Type == "id")?.Value;
        }
    }
}
