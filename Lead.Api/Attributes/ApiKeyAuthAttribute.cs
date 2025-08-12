using Application.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lead.Api.Attributes;


/// <summary>
/// Author : Md. Sakibur Rahman
/// Date   : 2025-08-12
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var headers = context.HttpContext.Request.Headers;

        // Get API Key from header
        if (!headers.TryGetValue("X-Business-Id", out var businessIdString) ||
            !headers.TryGetValue("X-Api-Key", out var providedApiKey))
        {
            context.Result = new ContentResult
            {
                StatusCode = 401,
                Content = "BusinessId or API key not provided."
            };
            return;
        }

        if (!int.TryParse(businessIdString, out var businessId))
        {
            context.Result = new ContentResult
            {
                StatusCode = 400,
                Content = "Invalid BusinessId."
            };
            return;
        }

        try
        {
            // Get service
            var apiKeyService = context.HttpContext.RequestServices.GetService<IApiKeyService>();
            if (apiKeyService == null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 500,
                    Content = "API Key service is not available."
                };
                return;
            }

            // Validate key
            var isValidKey = await apiKeyService.ValidateKey(businessId, providedApiKey);
            if (!isValidKey)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "Invalid API key."
                };
                return;
            }
        }
        catch
        {
            context.Result = new ContentResult
            {
                StatusCode = 400,
                Content = "Invalid request body format."
            };
            return;
        }

        await next(); // All good
    }
}
