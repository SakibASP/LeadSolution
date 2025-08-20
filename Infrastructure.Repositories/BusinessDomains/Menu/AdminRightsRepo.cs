using Core.Models.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Infrastructure.Interfaces.Menu;
using Infrastructure.Repositories.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Menu;

/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class AdminRightsRepo(LeadContext context) : IAdminRightsRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;
    private const int superAdminId = 100; // 100 -> Menu is only for developer,
    public async Task<string> CreateAsync(MenuItemViewModel viewMenuItemObj)
    {
        string message = string.Empty;

        if (string.IsNullOrEmpty(viewMenuItemObj.RoleId)) return "No role selected";

        var roleMenuMappingtblList = await _context.MenuToRole
                             .Where(w => w.RoleId == viewMenuItemObj.RoleId)
                             .ToListAsync();

        MenuToRole menuToRoleObj = new();

        foreach (var roleNMenu in viewMenuItemObj.MenuSelectionDto)
        {
            if (roleMenuMappingtblList.Count > 0)
            {
                var Id = roleMenuMappingtblList.Where(f => f.MenuId == roleNMenu.MenuId
                         && f.RoleId == viewMenuItemObj.RoleId).Select(s => s.Id).FirstOrDefault();

                var roleMenuMappingObjFromDb = roleMenuMappingtblList.Where(x => x.Id == Id)
                                               .FirstOrDefault();
                if (roleMenuMappingObjFromDb != null)
                {

                    roleMenuMappingObjFromDb.RoleId = viewMenuItemObj.RoleId;
                    roleMenuMappingObjFromDb.MenuId = roleNMenu.MenuId;
                    roleMenuMappingObjFromDb.Active = true;
                    roleMenuMappingObjFromDb.IsSelected = roleNMenu.IsSelected;

                    _context.Update(roleMenuMappingObjFromDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    menuToRoleObj.RoleId = viewMenuItemObj.RoleId;
                    menuToRoleObj.MenuId = roleNMenu.MenuId;
                    menuToRoleObj.Active = true;
                    menuToRoleObj.IsSelected = true;
                    await _context.AddAsync(menuToRoleObj);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {

                menuToRoleObj.RoleId = viewMenuItemObj.RoleId;
                menuToRoleObj.MenuId = roleNMenu.MenuId;
                menuToRoleObj.Active = true;
                menuToRoleObj.IsSelected = roleNMenu.IsSelected;

                await _context.AddAsync(menuToRoleObj);
                await _context.SaveChangesAsync();
            }
            message = "Rights updated successfuly!";
        }

        return message;
    }

    public async Task<string> UpdateRecordsAsync(UpdateAdminRightsRequest updateRequest)
    {
        string message = string.Empty;

        if (string.IsNullOrEmpty(updateRequest.RoleId)) return "No role selected";
        if (updateRequest.MenuSelections is null) return "No menu selected";

        var roleMenuMappingtblList = await _context.MenuToRole
                                         .Where(w => w.RoleId == updateRequest.RoleId)
                                         .ToListAsync();
        MenuToRole MenuToRoleObj = new();

        foreach (var roleNMenu in updateRequest.MenuSelections)
        {
            if (roleMenuMappingtblList.Count > 0)
            {
                var Id = roleMenuMappingtblList.Where(f => f.MenuId == roleNMenu.MenuId
                         && f.RoleId == updateRequest.RoleId).Select(s => s.Id).FirstOrDefault();

                var roleMenuMappingObjFromDb = roleMenuMappingtblList.Where(x => x.Id == Id)
                                               .FirstOrDefault();
                if (roleMenuMappingObjFromDb != null)
                {

                    roleMenuMappingObjFromDb.RoleId = updateRequest.RoleId;
                    roleMenuMappingObjFromDb.MenuId = roleNMenu.MenuId;
                    roleMenuMappingObjFromDb.Active = true;
                    roleMenuMappingObjFromDb.IsSelected = roleNMenu.IsSelected;

                    _context.Update(roleMenuMappingObjFromDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    MenuToRoleObj.RoleId = updateRequest.RoleId;
                    MenuToRoleObj.MenuId = roleNMenu.MenuId;
                    MenuToRoleObj.Active = true;
                    MenuToRoleObj.IsSelected = roleNMenu.IsSelected;
                    MenuToRoleObj.Id = null;

                    await _context.AddAsync(MenuToRoleObj);
                    await _context.SaveChangesAsync();
                }

            }
            else
            {
                MenuToRoleObj.RoleId = updateRequest.RoleId;
                MenuToRoleObj.MenuId = roleNMenu.MenuId;
                MenuToRoleObj.Active = true;
                MenuToRoleObj.IsSelected = roleNMenu.IsSelected;
                MenuToRoleObj.Id = null;

                await _context.AddAsync(MenuToRoleObj);
                await _context.SaveChangesAsync();
            }
            message = "Rights updated successfully";
        }
        return message;

    }

    public async Task<IList<MenuItem>> GetMenuMasterAsync(bool IsSuperAdmin)
    {
        IList<MenuItem> menuMaster = [];
        if (IsSuperAdmin)
            menuMaster = await _context.MenuItem.AsNoTracking().ToListAsync();
        else 
            menuMaster = await _context.MenuItem.AsNoTracking().Where(m => m.MenuId != superAdminId && m.MenuParentId != superAdminId).ToListAsync();

        IList<MenuItem> _menuMaster = [.. menuMaster
        .Where(e => e.MenuParentId == null) /* grab only the root parent nodes */
        .Select(e => new MenuItem
        {
            MenuId = e.MenuId,
            MenuName = e.MenuName,
            MenuParentId = e.MenuParentId,
            Children = GetChildren(menuMaster, e.MenuId) /* Recursively grab the children */
        })];

        return _menuMaster;
    }

    public async Task<IList<IdentityRole>> GetRoleListAsync(bool IsSuperAdmin)
    {
        var roles = await _context.Roles.AsNoTracking().ToListAsync();
        IList<IdentityRole> rolelist = [];
        if (IsSuperAdmin)
        {
            rolelist = [.. roles
                .OrderBy(r => r.Name)
                .ToList()];
        }
        else
        {
            rolelist = [.. roles.Where(r => !r.Name.Equals("SuperAdmin"))
               .OrderBy(r => r.Name)
               .ToList()];
        }
        return rolelist;
    }

    public async Task<IList<MenuItemViewModel>> GetRoleWiseSelectedPagesAsync(string roleId)
    {
        var menuMaster = await _context.MenuItem.AsNoTracking().ToListAsync();
        var menuToRoleViewModel = new MenuToRoleViewModel();
        List<MenuSelectionDto> menuSelections = [];

        var menuToRole = await _context.MenuToRole.AsNoTracking().ToListAsync();
        var roleMapppingResults = menuToRole.Join(menuMaster,     /// Inner Collection  
                                      r => r.MenuId,   /// PK  
                                      m => m.MenuId,   /// FK  
                                      (r, m) =>           /// Result Collection  
                                      new MenuItemViewModel
                                      {
                                          MenuSelectionDto = [
                                              new MenuSelectionDto() {
                                                  MenuId =r.MenuId,
                                                  MenuName =m.MenuName,
                                                  IsSelected =r.IsSelected ?? false,
                                                  MenuParentId=m.MenuParentId,
                                              }
                                          ],
                                          RoleId = r.RoleId
                                      }).ToList();

        var resultMenuToRole = roleMapppingResults.Where(w => w.RoleId == roleId).ToList();

        int count = 0;
        foreach (var item in resultMenuToRole)
        {
            foreach (var i in item.MenuSelectionDto)
            {
                i.Count = count;
            }
            count++;
        }

        return resultMenuToRole;
    }

    private List<MenuItem> GetChildren(IList<MenuItem> items, int parentId)
    {
        return [.. items
            .Where(x => x.MenuParentId == parentId)
            .Select(e => new MenuItem
            {
                MenuId = e.MenuId,
                MenuName = e.MenuName,
                MenuParentId = e.MenuParentId,
                Children = GetChildren(items, e.MenuId)
            })];
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
