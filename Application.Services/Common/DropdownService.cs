using Application.Interfaces.Common;
using Common.Utils.Enums;
using Common.Utils.Helper;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Application.Services.Common;

public class DropdownService(IDropdownRepo repo, IHttpContextAccessor httpContext) : IDropdownService
{
    private readonly IDropdownRepo _iDropRepo = repo;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    public async Task<ApiResponse<IList<DropdownDto>>> GetDropdownListAsync(DropdownRequest request)
    {
        try
        {
            var result = await _iDropRepo.GetDropdownListAsync(request);
            return ApiResponse<IList<DropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, request, CurrentUser));
            return ApiResponse<IList<DropdownDto>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<UserDropdownDto>>> GetUserDropdownListAsync(DropdownRequest request)
    {
        try
        {
            var result = await _iDropRepo.GetUserDropdownListAsync(request);
            return ApiResponse<IList<UserDropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(RequestPath, request, CurrentUser));
            return ApiResponse<IList<UserDropdownDto>>.Fail("Something went wrong!");
        }
    }
}
