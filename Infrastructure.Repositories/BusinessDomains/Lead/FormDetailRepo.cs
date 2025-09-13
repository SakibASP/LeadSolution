using Infrastructure.Interfaces.Lead;
using Infrastructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Lead;

public class FormDetailRepo(LeadContext context) : IFormDetailRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;
    public async Task RemoveAsync(int id)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Delete dependent records
            var supportedForms =  _context.BusinessSupportedFormId
                .Where(x => x.FormId == id);
            var messages = _context.FormValueDetails
                .Where(x => x.FormId == id);

            if (await supportedForms.AnyAsync()) _context.BusinessSupportedFormId.RemoveRange(supportedForms);
            if (await messages.AnyAsync()) _context.FormValueDetails.RemoveRange(messages);

            // Delete the main record
            var formDetail = await _context.FormDetails.FindAsync(id);
            if (formDetail is not null) _context.FormDetails.Remove(formDetail);

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
