using Common.Utils.Constant;
using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Dapper;
using Infrastructure.Interfaces.Common;
using Infrastructure.Interfaces.Lead;
using Infrastructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace Infrastructure.Repositories.BusinessDomains.Lead;

public class FormValueRepo(LeadContext context, IDapperContext dapper) : IFormValueRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;
    private readonly IDapperContext _dapper = dapper;

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

    public async Task<DynamicFormViewModel> GetDynamicFormAsync(int? businessId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@BusinessId", businessId, DbType.Int32);

        using var connection = _dapper.CreateConnection();
        var result = await connection.QueryAsync<DynamicFormDto>(
            Sp.usp_GetBusinessSupportedForms,
            parameters,
            commandType: CommandType.StoredProcedure);


        return new DynamicFormViewModel
        {
            Inputs = [.. result],
        };
    }

    public async Task UpdateFormSettingsAsync(UpdateFormSettingsRequest request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@BusinessId", request.BusinessId, DbType.Int32);
        parameters.Add("@Username", request.Username ?? string.Empty, DbType.String);

        // Serialize FormSelectDetails to JSON string
        var json = JsonSerializer.Serialize(new
        {
            FormSelectDetails = request.FormSelectDetails ?? []
        });
        parameters.Add("@JsonObject", json, DbType.String);

        using var connection = _dapper.CreateConnection();

        await connection.ExecuteAsync(
            "dbo.usp_InsertUpdateBusinessSupportedForms",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

}
