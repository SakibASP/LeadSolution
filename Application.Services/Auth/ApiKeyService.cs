using Application.Interfaces.Auth;
using Common.Extentions;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Auth;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Auth;

public class ApiKeyService(IApiKeyRepo apiKeyRepo, IHttpContextAccessor httpContext) : IApiKeyService
{
    private readonly IApiKeyRepo _iRepo = apiKeyRepo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";
    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";

    public async Task<ApiResponse<string?>> GenerateKeyByBusinessId(int businessId)
    {
        try
        {
            string key = GenerateKey(businessId);
            AspNetBusinessApiKeys businessApiKeys = new()
            {
                BusinessId = businessId,
                ApiKey = key,
                CreatedBy = CurrentUser
            };
            await _iRepo.GenerateNewApiKeyAsync(businessApiKeys);
            return ApiResponse<string?>.Success(EncryptionHelper.Encrypt(key), "Key generated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error generating Apikey: {@businessId}", businessId);
            return ApiResponse<string?>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<string?>> GetKeyByBusinessId(int businessId)
    {
        try
        {
            var apiKey = await _iRepo.GetActiveApiKeyAsync(businessId);
            return ApiResponse<string?>.Success(EncryptionHelper.Encrypt(apiKey?.ApiKey), "Key got successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting Apikey: {@businessId}", businessId);
            return ApiResponse<string?>.Fail("Something went wrong!");
        }
    }

    public async Task<bool> ValidateKey(int businessId, string? key)
    {
        try
        {
            return await _iRepo.ValidateKey(businessId, key ?? "");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error validate Apikey: {@businessId}, {@key}", businessId, key);
            return false;
        }
    }

    private static string GenerateKey(int businessId)
    {
        var random = new Random();
        int code = random.Next(1000, 10000);  // Generates a number between 1000 and 9999 inclusive
        string currentTime = DateTime.Now.ToBangladeshTime().ToString("yyyyMMddHHmmss");
        return businessId.ToString() + '-' + code.ToString() + '-' + currentTime;
    }
}
