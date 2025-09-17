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


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class FormValueRepo(LeadContext context, IDapperContext dapper) : IFormValueRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;
    private readonly IDapperContext _dapper = dapper;

    //public async Task<dynamic> GetMessagesByBusinessAsync(GetFormValueRequest request)
    //{
    //    var parameters = new DynamicParameters();
    //    parameters.Add("@BusinessId", request.BusinessId);
    //    parameters.Add("@FromDate", request.FromDate.Date);
    //    parameters.Add("@ToDate", request.ToDate.Date);

    //    using var connection = _dapper.CreateConnection();
    //    var result = await connection.QueryAsync<dynamic>(
    //        Sp.usp_GetDynamicPivotedFormValues,
    //        parameters,
    //        commandType: CommandType.StoredProcedure);
    //    return result.ToList();
    //}

    public async Task<IList<FormValueMaster>> GetMessagesByBusinessAsync(GetFormValueRequest request)
    {
        return await _context.FormValueMaster
            .Where(x => x.BusinessId == request.BusinessId)
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<IList<FormValueViewModel>> GetMessagesDetailsByMasterIdAsync(int masterId)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@MasterId", masterId);
        using var connection = _dapper.CreateConnection();
        var result = await connection.QueryAsync<FormValueViewModel>(
            Sp.usp_GetFormValuesByMasterId,
            parameters,
            commandType: CommandType.StoredProcedure);
        return [.. result];
    }



    /// <summary>
    /// Adding Messages to Database
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> AddAsync(DynamicFormViewModel model)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
        try
        {
            IList<FormValueDetails> formValues = [];
            if (model.Inputs == null || model.Inputs.Count == 0) return false;
            // Get the next submission ID for the business
            var submissionId = (await _context.FormValueMaster
                .AsNoTracking()
                .Where(x => x.BusinessId == model.BusinessId)
                .MaxAsync(x => x.SubmissionId) ?? 0) + 1;
            // Count non-null inputs
            var totalInputs = model.Inputs.Where(x => x != null).Count();

            // Create FormValueMaster entry
            var formValueMaster = new FormValueMaster
            {
                BusinessId = model.BusinessId,
                SubmissionId = submissionId,
                TotalItems = totalInputs,
                IpAddress = model.IpAddress
            };
            await _context.FormValueMaster.AddAsync(formValueMaster);
            await _context.SaveChangesAsync();

            // Prepare FormValueDetails entries
            foreach (var input in model.Inputs)
            {
                if (input == null) continue;
                formValues.Add(new FormValueDetails
                {
                    FormId = input.FormDetailId,
                    FormValue = input.Value,
                    FormMasterId = formValueMaster.Id
                });
            }

            if (formValues.Count == 0) return false;

            await _context.FormValueDetails.AddRangeAsync(formValues);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    #region - form settings -
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
    #endregion
    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

}
