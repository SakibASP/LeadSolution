

namespace Lead.Api.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Log request method and path
        Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");

        // Optional: log headers
        foreach (var header in context.Request.Headers)
        {
            Console.WriteLine($"{header.Key}: {header.Value}");
        }

        // Optional: read request body (if needed)
        if (context.Request.ContentLength > 0 &&
            context.Request.Body.CanSeek)
        {
            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            Console.WriteLine("Body:");
            Console.WriteLine(body);
            context.Request.Body.Position = 0;
        }

        // Call the next middleware
        await _next(context);

        // Log response status code
        Console.WriteLine($"Response Status: {context.Response.StatusCode}");
    }
}
