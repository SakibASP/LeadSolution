using Application.Interfaces.Auth;
using Common.Utils.Helper;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Lead.Api.Filters;

/// <summary>
/// Author : Md. Sakibur Rahman
/// Date   : 2025-08-12
/// </summary>
public class ApiKeyAuthFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpContext = context.HttpContext;
        var headers = httpContext.Request.Headers;

        // Validate headers
        if (!headers.TryGetValue("X-Business-Id", out var businessIdString) ||
            !headers.TryGetValue("X-Api-Key", out var providedApiKey))
        {
            WriteLog("BusinessId or API key not provided.", httpContext);
            return Results.Json(new
            {
                success = false,
                message = "BusinessId or API key not provided."
            }, statusCode: StatusCodes.Status401Unauthorized);
        }

        if (!int.TryParse(businessIdString, out var businessId))
        {
            WriteLog("Invalid BusinessId.", httpContext);
            return Results.Json(new
            {
                success = false,
                message = "Invalid BusinessId."
            }, statusCode: StatusCodes.Status400BadRequest);
        }

        // Resolve the API key service
        var apiKeyService = httpContext.RequestServices.GetService<IApiKeyService>();
        if (apiKeyService == null)
        {
            WriteLog("API Key service is not available.", httpContext);
            return Results.Json(new
            {
                success = false,
                message = "API Key service is not available."
            }, statusCode: StatusCodes.Status500InternalServerError);
        }

        // Validate key
        var isValidKey = await apiKeyService.ValidateKey(businessId, providedApiKey);
        if (!isValidKey)
        {
            WriteLog("Invalid API key.", httpContext);
            return Results.Json(new
            {
                success = false,
                message = "Invalid API key."
            }, statusCode: StatusCodes.Status401Unauthorized);
        }

        // Continue to the next middleware / endpoint
        return await next(context);
    }

    private static void WriteLog(string message, HttpContext? context)
    {
        Log
               .ForContext("UserName", IpAddressHelper.GetClientIpAddress(context))
               .ForContext("Path", "ApiKeyAuthFilter")
               .Warning(message);
    }
}
