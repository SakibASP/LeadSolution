using Core.ViewModels.Dto.Menu;
using Infrustructure.Interfaces.Menu;
using Infrustructure.Repositories.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrustructure.Repositories.Menu;

/// <summary>
/// Repository for menu-related data access operations.
/// </summary>
public class MenuRepo(LeadContext context) : IMenuRepo
{
    private readonly LeadContext _context = context;

    /// <summary>
    /// Retrieves all dynamic menu items available for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user for whom to fetch menu items. If null, returns menu items for all users or default menu.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of <see cref="DynamicMenuItemDto"/> objects.
    /// </returns>
    public async Task<IList<DynamicMenuItemDto>> GetAllMenuAsync(string? userId)
    {
        return await _context.Database.SqlQueryRaw<DynamicMenuItemDto>(
            "exec usp_GetMenuData @UserId",
            new SqlParameter("UserId", userId)
        )
        .AsNoTracking()
        .ToListAsync();
    }
}
