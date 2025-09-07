using Common.Extentions;
using Core.Models.Common;
using Infrastructure.Interfaces.Common;
using Serilog;
using System.Text;

namespace Lead.Api.Middlewares;

/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>
public class RequestLoggingMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
{
    private readonly RequestDelegate _next = next;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private const int MaxBodySize = 1024 * 1024; // 1MB limit

    public async Task InvokeAsync(HttpContext context)
    {
        string userId = "Unauthorized";
        string? requestBody = null;
        string? responseText = null;
        int statusCode = 500;

        try
        {
            // Extract user ID from JWT
            userId = ExtractUserIdFromToken(context);

            // Read request body
            requestBody = await ReadRequestBodyAsync(context.Request);

            // Capture response
            var (capturedResponse, actualStatusCode) = await CaptureResponseAsync(context);
            responseText = capturedResponse;
            statusCode = actualStatusCode;
        }
        catch (Exception ex)
        {
            statusCode = 500;
            responseText = $"[Middleware Error] {ex.Message}";
            Log.Error(ex, "[RequestLoggingMiddleware]: Unhandled exception in {Path}", context.Request.Path);
        }
        finally
        {
            await LogRequestAsync(context, userId, requestBody, responseText, statusCode);
        }
    }

    #region - log write business logic -
    private static string ExtractUserIdFromToken(HttpContext context)
    {
        try
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return "Unauthorized";

            var token = authHeader["Bearer ".Length..].Trim();
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            return jwtToken.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                   ?? jwtToken.Claims.FirstOrDefault(c =>
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
                   ?? "Unknown";
        }
        catch
        {
            return "InvalidToken";
        }
    }

    private static async Task<string?> ReadRequestBodyAsync(HttpRequest request)
    {
        try
        {
            // Skip if no body or unsupported content type
            if (request.ContentLength == 0 || !IsReadableContentType(request.ContentType))
            {
                // Even if body is empty, include query params
                if (request.Query.Count != 0)
                {
                    return "QueryParams: " + string.Join("&", request.Query.Select(q => $"{q.Key}={q.Value}"));
                }
                return null;
            }

            // Limit body size to prevent memory issues
            if (request.ContentLength > MaxBodySize)
                return $"[Request body too large: {request.ContentLength} bytes]";

            // Enable buffering to allow multiple reads
            request.EnableBuffering();

            // Read the body
            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 1024,
                leaveOpen: true);

            var body = $"Body: {await reader.ReadToEndAsync()}";

            // Reset stream position for the next middleware
            request.Body.Position = 0;

            // Append query parameters if any
            if (request.Query.Count != 0)
            {
                var queryParams = string.Join("&", request.Query.Select(q => $"{q.Key}={q.Value}"));
                if (!string.IsNullOrWhiteSpace(body))
                    body += $"\nQueryParams: {queryParams}";
                else
                    body = $"QueryParams: {queryParams}";
            }

            return string.IsNullOrWhiteSpace(body) ? null : body;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Failed to read request body for {Method} {Path}", request.Method, request.Path);
            return $"[Error reading request body: {ex.Message}]";
        }
    }

    private async Task<(string? responseBody, int statusCode)> CaptureResponseAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Call the next middleware
            await _next(context);

            var statusCode = context.Response.StatusCode;

            // Read response body
            responseBody.Seek(0, SeekOrigin.Begin);
            string? responseText = null;

            if (responseBody.Length > 0 && responseBody.Length <= MaxBodySize)
            {
                using var reader = new StreamReader(responseBody, Encoding.UTF8, leaveOpen: true);
                responseText = await reader.ReadToEndAsync();
                responseBody.Seek(0, SeekOrigin.Begin);
            }
            else if (responseBody.Length > MaxBodySize)
            {
                responseText = $"[Response body too large: {responseBody.Length} bytes]";
            }

            // Copy the response back to the original stream
            await responseBody.CopyToAsync(originalBodyStream);

            return (responseText, statusCode);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error capturing response for {Method} {Path}", context.Request.Method, context.Request.Path);
            return ($"[Error capturing response: {ex.Message}]", 500);
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private async Task LogRequestAsync(HttpContext context, string userId, string? requestBody, string? responseBody, int statusCode)
    {
        try
        {
            if (IsLoggablePath(context.Request.Path))
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IGenericRepo<RequestLogs>>();

                var log = new RequestLogs
                {
                    UserId = userId,
                    Method = context.Request.Method,
                    Path = context.Request.Path,
                    Request = requestBody,
                    StatusCode = statusCode,
                    Response = responseBody,
                    CreatedDate = DateTime.Now.ToBangladeshTime()
                };

                await repo.AddAsync(log);
            }

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to save request log for {Method} {Path} by user {UserId}",
                context.Request.Method, context.Request.Path, userId);
        }
    }

    private static bool IsReadableContentType(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return false;

        var lowerContentType = contentType.ToLowerInvariant();

        return lowerContentType.Contains("application/json") ||
               lowerContentType.Contains("application/xml") ||
               lowerContentType.Contains("text/") ||
               lowerContentType.Contains("application/x-www-form-urlencoded") ||
               lowerContentType.Contains("multipart/form-data");
    }

    private static bool IsLoggablePath(string path)
    {
        Span<string> avoidUrls = [ "/api/v1/menu/get-menu-by-user", "/health/details"];
        return !avoidUrls.Contains(path);
    }

    #endregion
}