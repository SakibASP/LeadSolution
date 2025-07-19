using Application.Interfaces.Menu;
using Core.Models.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Core.ViewModels.Response;
using Infrustructure.Interfaces.Menu;
using Microsoft.AspNetCore.Identity;

namespace Application.Services.Menu;

public class AdminRightsService(IAdminRightsRepo adminRights) : IAdminRightsService
{
    private readonly IAdminRightsRepo _iAdminRights = adminRights;
    public async Task<ApiResponse<string>> CreateAsync(MenuItemViewModel viewMenuItemObj)
    {
        var data = await _iAdminRights.CreateAsync(viewMenuItemObj);
        if (string.IsNullOrEmpty(data)) return ApiResponse<string>.Fail("No items found.");
        return ApiResponse<string>.Success(data,"Rights updated successfully!");
    }

    public async Task<ApiResponse<string>> UpdateRecordsAsync(UpdateAdminRightsRequest updateRequest)
    {
        var data = await _iAdminRights.UpdateRecordsAsync(updateRequest);
        if (string.IsNullOrEmpty(data)) return ApiResponse<string>.Fail("No items found.");
        return ApiResponse<string>.Success(data, "Rights updated successfully!");
    }

    public async Task<ApiResponse<IList<MenuItem>>> GetMenuMasterAsync(bool IsSuperAdmin)
    {
        var data = await _iAdminRights.GetMenuMasterAsync(IsSuperAdmin);
        if (data is null || !data.Any()) return ApiResponse<IList<MenuItem>>.Fail("No items found.");
        return ApiResponse<IList<MenuItem>>.Success(data);
    }

    public async Task<ApiResponse<IList<IdentityRole>>> GetRoleListAsync(bool IsSuperAdmin)
    {
        var data = await _iAdminRights.GetRoleListAsync(IsSuperAdmin);
        if (data is null || !data.Any()) return ApiResponse<IList<IdentityRole>>.Fail("No items found.");
        return ApiResponse<IList<IdentityRole>>.Success(data);
    }

    public async Task<ApiResponse<IList<MenuItemViewModel>>> GetRoleWiseSelectedPagesAsync(string roleId)
    {
        var data = await _iAdminRights.GetRoleWiseSelectedPagesAsync(roleId);
        if (data is null || !data.Any()) return ApiResponse<IList<MenuItemViewModel>>.Fail("No items found.");
        return ApiResponse<IList<MenuItemViewModel>>.Success(data);
    }
}
