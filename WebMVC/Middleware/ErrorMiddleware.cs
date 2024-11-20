using System.Net;

namespace WebMVC.Middleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    context.Response.Redirect("/Error/Forbidden");
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    context.Response.Redirect("/Error/NotFound");
                }
                return Task.CompletedTask;
            });
            await _next(context);
        }
    }
}
