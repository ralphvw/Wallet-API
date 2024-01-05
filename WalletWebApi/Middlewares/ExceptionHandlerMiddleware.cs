using System.Net;

namespace WalletWebApi.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            var errorId = Guid.NewGuid();
            _logger.LogError(e, $"{errorId}: {e.Message}");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var error = new
            {
                Id = errorId,
                ErrorMessage = "Something went wrong."
            };

            await context.Response.WriteAsJsonAsync(error);
        }
    }
}