using Core.ViewModels.Dto.Menu;
using Infrastructure.Interfaces.Menu;
using Infrastructure.Repositories.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Menu;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class MenuRepo(LeadContext context) : IMenuRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;

    /// <summary>
    /// Retrieves all dynamic menu items available for the specified user.
    /// </summary>
    public async Task<IList<DynamicMenuItemDto>> GetAllMenuAsync(string? userId)
    {
        return await _context.Database.SqlQueryRaw<DynamicMenuItemDto>(
            "exec usp_GetMenuData @UserId",
            new SqlParameter("UserId", userId)
        )
        .AsNoTracking()
        .ToListAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
