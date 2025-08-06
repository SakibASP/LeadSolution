using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Infrastructure.Interfaces.Lead;
using Infrastructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Lead;

public class FormValueRepo(LeadContext context) : IFormValueRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;

    public async Task<IList<FormValues>> GetAllAsync(dynamic? param = null)
    {
        return await _context.FormValues
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(DynamicFormViewModel model)
    {
        IList<FormValues> formValues = [];
        if (model.Inputs == null) return;
        var submissionId = (await _context.FormValues.MaxAsync(x => x.SubmissionId) ?? 0) + 1;
        foreach (var input in model.Inputs)
        {
            if (input == null) continue;
            formValues.Add(new FormValues
            {
                FormId = input.FormDetailId,
                FormValue = input.Value,
                BusinessId = model.BusinessId,
                SubmissionId = submissionId
            });
        }

        if (formValues.Count == 0) return;

        await _context.FormValues.AddRangeAsync(formValues);
        await _context.SaveChangesAsync();
    }

    public async Task<DynamicFormViewModel> GetDynamicFormAsync(dynamic? param = null)
    {
        var formDetails = await _context.FormDetails
            .AsNoTracking()
            .Where(x=>x.IsActive)
            .ToListAsync();

        return new DynamicFormViewModel
        {
            Inputs = [.. formDetails.Select(f => new DynamicFormDto
            {
                FormDetailId = f.Id,
                Label = f.Name,
                InputType = f.DataTypes?.Name ?? "text",
                IsSelectInput = f.IsSelectInput
            })]
        };
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

}
