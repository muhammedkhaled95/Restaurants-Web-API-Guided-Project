using Restaurants.Domain.Exceptions;

namespace Restaurants.API.Middlewares
{
    /// <summary>
    /// Middleware for global error handling in the application.
    /// Catches exceptions thrown in subsequent middlewares or request handlers,
    /// logs the error, and returns a generic error response.
    /// </summary>
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="logger">Logger used to log exception details.</param>
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Processes an HTTP request by invoking the next delegate in the pipeline.
        /// If an exception occurs, it logs the error and returns a generic error response.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <param name="next">The next delegate/middleware in the request pipeline.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                // Invoke the next middleware in the pipeline.
                await next.Invoke(context);
            }
            // catching a custom made exception for resources that were not found.
            catch (ResourceNotFoundException notFoundEx)
            {
                _logger.LogWarning(notFoundEx, notFoundEx.Message);

                context.Response.StatusCode = 404;

                await context.Response.WriteAsync(notFoundEx.Message);
            }
            catch (ForbidException fex)
            {
                _logger.LogError(fex, fex.Message);
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Access to resource is forbidden");
            }
            catch (Exception ex)
            {
                // Log the exception with its message.
                _logger.LogError(ex, ex.Message);

                // Set the response status code to 500 (Internal Server Error).
                context.Response.StatusCode = 500;

                // Write a generic error message to the response.
                await context.Response.WriteAsync("Something went wrong");
            }
        }
    }
}
