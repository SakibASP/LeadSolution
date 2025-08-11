using Core.Models.Auth;
using Core.ViewModels.Response;

namespace Application.Interfaces.Auth;

public interface IApiKeyService
{
    Task<ApiResponse<string?>> GetKeyByBusinessId(int businessId);
    Task<ApiResponse<string?>> GenerateKeyByBusinessId(int businessId);
    Task<bool> ValidateKey(int businessId, string? key);
}
