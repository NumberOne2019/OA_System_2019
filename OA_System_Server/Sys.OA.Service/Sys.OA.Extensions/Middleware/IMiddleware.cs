using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sys.OA.Extensions.Middleware
{
    public interface IMiddleware
    {
        Task InvokeAsync(HttpContext context,RequestDelegate next);
    }
}
