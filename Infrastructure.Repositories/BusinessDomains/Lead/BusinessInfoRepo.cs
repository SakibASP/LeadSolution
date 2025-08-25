using Core.Models.Auth;
using Core.ViewModels.Dto.Lead;
using Infrastructure.Interfaces.Lead;
using Infrastructure.Repositories.Data;

namespace Infrastructure.Repositories.BusinessDomains.Lead;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class BusinessInfoRepo(LeadContext context) : IBusinessInfoRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;
    public async Task AddAsync(AspNetBusinessInfoDto businessInfo)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            AspNetBusinessInfo aspNetBusinessInfo = new()
            {
                ServiceId = businessInfo.ServiceId,
                BusinessName = businessInfo.BusinessName,
                Phone = businessInfo.Phone,
                Email = businessInfo.Email,
                WhatsApp = businessInfo.WhatsApp,
                Website = businessInfo.Website,
                Address = businessInfo.Address,
                IsActive = true,
                CreatedBy = businessInfo.UserName
            };
            await _context.AddAsync(aspNetBusinessInfo);
            await _context.SaveChangesAsync();

            AspNetUserBusinessInfo aspNetUserBusinessInfo = new()
            {
                BusinessId = aspNetBusinessInfo.Id,
                UserId = businessInfo.UserId,
                CreatedBy = businessInfo.UserName
            };

            await _context.AddAsync(aspNetUserBusinessInfo);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateAsync(AspNetBusinessInfo aspNetBusinessInfo)
    {
        _context.Update(aspNetBusinessInfo);
        await _context.SaveChangesAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
