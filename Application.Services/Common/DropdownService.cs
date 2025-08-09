using Application.Interfaces.Common;
using Common.Utils.Enums;
using Common.Utils.Helper;
using Core.Models.Lead;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Security.Claims;

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
            // if the dropdown is 'user wise business' then it takes the related dropdown using the user id
            if (request.Id == (int)DropdownEnum.UserWiseBusinesses) 
                request.Param1 = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _iDropRepo.GetDropdownListAsync<DropdownDto>(request);
            return ApiResponse<IList<DropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting dropdown: {@request}", request);
            return ApiResponse<IList<DropdownDto>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<UserDropdownDto>>> GetUserDropdownListAsync(DropdownRequest request)
    {
        try
        {
            var result = await _iDropRepo.GetDropdownListAsync<UserDropdownDto>(request);
            return ApiResponse<IList<UserDropdownDto>>.Success(result, string.Empty);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error getting dropdown: {@request}", request);
            return ApiResponse<IList<UserDropdownDto>>.Fail("Something went wrong!");
        }
    }
}
