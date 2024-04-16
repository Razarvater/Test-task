using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace WorldWord.Api.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter, IOrderedFilter
    {
        public int Order => 1;
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            await Console.Out.WriteLineAsync(context.Exception.ToString());
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = 500;
            await context.HttpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes("unexpected-error"));
            context.ExceptionHandled = true;
        }
    }
}
