namespace NotificationService.Middleware;

public class ValidationLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationLoggingMiddleware> _logger;

    public ValidationLoggingMiddleware(RequestDelegate next, ILogger<ValidationLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

        if (context.Response.StatusCode == 400)
        {
            _logger.LogWarning("Validation failed for {Method} {Path} — response: {Response}",
                context.Request.Method,
                context.Request.Path,
                responseBody);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        await memoryStream.CopyToAsync(originalBody);
        context.Response.Body = originalBody;
    }
}