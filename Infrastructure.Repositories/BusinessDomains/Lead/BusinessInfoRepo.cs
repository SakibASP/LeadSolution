using Common.Utils.Constant;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Lead;
using Infrastructure.Interfaces.Lead;
using Infrastructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Lead;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class BusinessInfoRepo(LeadContext context) : IBusinessInfoRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;

    public async Task<IList<AspNetBusinessInfo>> GetAsync(UserInfoViewModel parameter)
    {
        if (parameter.UserRoles?.Contains(Constants.Admin) is true
            || parameter.UserRoles?.Contains(Constants.SuperAdmin) is true)
        {
            return await _context.AspNetBusinessInfo
                .AsNoTracking()
                .ToListAsync();
        }
        else
        {
            string? userId = parameter.UserId;
            var supportedBusinessIds = await _context.AspNetUserBusinessInfo
                .AsNoTracking()
                .Where(ub => ub.UserId == userId)
                .Select(ub => ub.BusinessId)
                .ToListAsync();
            return await _context.AspNetBusinessInfo
                .AsNoTracking()
                .Where(b => supportedBusinessIds.Contains(b.Id))
                .ToListAsync();
        }
    }


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

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

}
