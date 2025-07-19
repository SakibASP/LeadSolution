using Core.Models.Auth;
using Infrustructure.Interfaces.Auth;
using Infrustructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrustructure.Repositories.BusinessDomains.Auth;

public class ServiceTypeRepo(LeadContext context) : IServiceTypeRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;
    public async Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync() 
        => await _context.AspNetServiceTypes
        .AsNoTracking()
        .Where(x => x.IsActive)
        .ToListAsync();

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
