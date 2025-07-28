using Core.Models.Auth;
using Infrastructure.Interfaces.Lead;
using Infrastructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Lead;

public class ServiceTypeRepo(LeadContext context) : IServiceTypeRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;

    public async Task<IList<AspNetServiceTypes>> GetAllAsync() => await _context.AspNetServiceTypes
        .AsNoTracking()
        .Where(x => x.IsActive)
        .ToListAsync();

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
