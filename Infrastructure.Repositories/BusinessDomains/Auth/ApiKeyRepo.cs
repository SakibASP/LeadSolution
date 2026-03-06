using Common.Utils.Extentions;
using Common.Utils.Helper;
using Core.Models.Auth;
using Infrastructure.Interfaces.Auth;
using Infrastructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Encryptor = EncryptionHelper.EncryptionHelper;

namespace Infrastructure.Repositories.BusinessDomains.Auth;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class ApiKeyRepo(LeadContext context) : IApiKeyRepo
{
    private readonly LeadContext _context = context;
    public async Task GenerateNewApiKeyAsync(AspNetBusinessApiKeys aspNetBusinessApiKeys)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var apikey = await _context.AspNetBusinessApiKeys.FirstOrDefaultAsync(x => x.BusinessId == aspNetBusinessApiKeys.BusinessId && x.IsActive);
            if (apikey is not null)
            {
                apikey.IsActive = false;
                apikey.ModifiedDate = DateTime.Now.ToBangladeshTime();
                apikey.ModifiedBy = aspNetBusinessApiKeys.CreatedBy;
                _context.Update(apikey);
                await _context.SaveChangesAsync();
            }
            await _context.AddAsync(aspNetBusinessApiKeys);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error generating new API key", ex);
        }
    }

    public async Task<AspNetBusinessApiKeys?> GetActiveApiKeyAsync(int businessId)
    {
        return await _context.AspNetBusinessApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessId == businessId && x.IsActive);
    }

    public async Task<bool> ValidateKey(int businessId, string key)
    {
        var keyToCheck = Encryptor.Decrypt(key);
        return await _context.AspNetBusinessApiKeys
            .AnyAsync(x => x.BusinessId == businessId && x.ApiKey == keyToCheck && x.IsActive);
    }
}
