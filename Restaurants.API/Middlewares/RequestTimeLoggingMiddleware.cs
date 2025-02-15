
using System.Diagnostics;

namespace Restaurants.API.Middlewares;

public class RequestTimeLoggingMiddleware : IMiddleware
{   
    private readonly ILogger<RequestTimeLoggingMiddleware> _logger;
    public RequestTimeLoggingMiddleware(ILogger<RequestTimeLoggingMiddleware> logger)
    {
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Start timer
        Stopwatch sw = Stopwatch.StartNew();

        await next.Invoke(context);

        // Stop timer
        sw.Stop();

        if (sw.ElapsedMilliseconds > 4000) 
        {
            _logger.LogInformation("Request [{verb}] at {path} took {time} ms",
                context.Request.Method,
                context.Request.Path,
                sw.ElapsedMilliseconds);
        }
    }
}
