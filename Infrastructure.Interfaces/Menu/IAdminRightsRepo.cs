using Core.Models.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Interfaces.Menu;

public interface IAdminRightsRepo
{
    /// <summary>
    /// it will return user wise role list
    /// </summary>
    Task<IList<IdentityRole>> GetRoleListAsync(bool IsSuperAdmin);

    /// <summary>
    /// it will return user wise menulist
    /// </summary>
    Task<IList<MenuItem>> GetMenuMasterAsync(bool IsSuperAdmin);

    /// <summary>
    /// it will return all the rights including selected and not selected to the ui
    /// </summary>
    Task<IList<MenuItemViewModel>> GetRoleWiseSelectedPagesAsync(string roleId);

    /// <summary>
    /// It will create new rights of a role
    /// </summary>
    Task<string> CreateAsync(MenuItemViewModel viewMenuItemObj);

    /// <summary>
    /// It will update the rights of a role
    /// </summary>
    Task<string> UpdateRecordsAsync(UpdateAdminRightsRequest updateRequest);

}
