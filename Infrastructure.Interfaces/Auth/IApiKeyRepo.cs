using Core.Models.Auth;

namespace Infrastructure.Interfaces.Auth;

public interface IApiKeyRepo
{
    Task<AspNetBusinessApiKeys?> GetActiveApiKeyAsync(int businessId);
    Task GenerateNewApiKeyAsync(AspNetBusinessApiKeys aspNetBusinessApiKeys);
    Task<bool> ValidateKey(int businessId, string key);
}
