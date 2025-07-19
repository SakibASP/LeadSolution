using Core.Models.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces.Menu;

public interface IAdminRightsService
{
    /// <summary>
    /// it will return user wise role list
    /// </summary>
    Task<ApiResponse<IList<IdentityRole>>> GetRoleListAsync(bool IsSuperAdmin);

    /// <summary>
    /// it will return user wise menulist
    /// </summary>
    Task<ApiResponse<IList<MenuItem>>> GetMenuMasterAsync(bool IsSuperAdmin);

    /// <summary>
    /// it will return all the rights including selected and not selected to the ui
    /// </summary>
    Task<ApiResponse<IList<MenuItemViewModel>>> GetRoleWiseSelectedPagesAsync(string roleId);

    /// <summary>
    /// It will create new rights of a role
    /// </summary>
    Task<ApiResponse<string>> CreateAsync(MenuItemViewModel viewMenuItemObj);

    /// <summary>
    /// It will update the rights of a role
    /// </summary>
    Task<ApiResponse<string>> UpdateRecordsAsync(UpdateAdminRightsRequest updateRequest);
}
