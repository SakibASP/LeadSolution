using Core.Models.Auth;
using Infrustructure.Interfaces.Auth;
using Infrustructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrustructure.Repositories.Auth
{
    public class ServiceTypeRepo(LeadContext context) : IServiceTypeRepo
    {
        private readonly LeadContext _context = context;
        public async Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync() => await _context.AspNetServiceTypes
            .AsNoTracking()
            .Where(x=>x.IsActive)
            .ToListAsync();
    }
}
