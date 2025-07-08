using Core.ViewModels.Dto.Menu;
using Infrustructure.Interfaces.Menu;
using Infrustructure.Repositories.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrustructure.Repositories.Menu
{
    public class MenuRepo(LeadContext context) : IMenuRepo
    {
        private readonly LeadContext _context = context;
        public async Task<IList<DynamicMenuItemDto>> GetAllMenuAsync(string? userId)
        {
            return await _context.Database.SqlQueryRaw<DynamicMenuItemDto>("exec usp_GetMenuData @UserId", new SqlParameter("UserId", userId)).ToListAsync();
        }
    }
}
