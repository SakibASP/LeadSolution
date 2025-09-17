using Application.Interfaces.BgQueue;
using Application.Interfaces.Common;
using Application.Interfaces.Lead;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.Models.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Core.ViewModels.Response;
using Infrastructure.Interfaces.Common;
using Infrastructure.Interfaces.Lead;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services.Lead;

public class FormValueService(IFormValueRepo repo, 
    IHttpContextAccessor httpContext,
    IGenericRepo<AspNetBusinessInfo> businessInfo,
    IGenericRepo<FormDetails> formDetail,
    IBackgroundTaskQueue backgroundTaskQueue) : IFormValueService
{
    private readonly IFormValueRepo _iFormValue = repo;
    private readonly IHttpContextAccessor _httpContext = httpContext;
    private readonly IGenericRepo<AspNetBusinessInfo> _iGenericBusinessInfo = businessInfo;
    private readonly IGenericRepo<FormDetails> _iGenericFromDetail = formDetail;
    private readonly IBackgroundTaskQueue _iTaskQueue = backgroundTaskQueue;

    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";
    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";

    //public async Task<ApiResponse<dynamic>> GetMessagesByBusinessAsync(GetFormValueRequest request)
    //{
    //    try
    //    {
    //        var result = await _iFormValue.GetMessagesByBusinessAsync(request);
    //        return ApiResponse<dynamic>.Success(result, "Messages retrieved successfully!");
    //    }
    //    catch (Exception ex)
    //    {
    //        Log
    //            .ForContext("UserName", CurrentUser)
    //            .ForContext("Path", RequestPath)
    //            .Error(ex, "Error retrieving messages for GetFormValueRequest: {request}", JsonSerializer.Serialize(request));
    //        return ApiResponse<dynamic>.Fail("Something went wrong!");
    //    }
    //}

    public async Task<ApiResponse<IList<FormValueMaster>>> GetMessagesByBusinessAsync(GetFormValueRequest request)
    {
        try
        {
            var result = await _iFormValue.GetMessagesByBusinessAsync(request);
            return ApiResponse<IList<FormValueMaster>>.Success(result, "Messages retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving messages for GetMessagesByBusinessAsync: {request}", JsonSerializer.Serialize(request));
            return ApiResponse<IList<FormValueMaster>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<IList<FormValueViewModel>>> GetMessagesDetailsByMasterIdAsync(int masterId)
    {
        try
        {
            var result = await _iFormValue.GetMessagesDetailsByMasterIdAsync(masterId);
            return ApiResponse<IList<FormValueViewModel>>.Success(result, "Messages retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving messages for GetMessagesDetailsByMasterIdAsync: {masterId}", masterId);
            return ApiResponse<IList<FormValueViewModel>>.Fail("Something went wrong!");
        }
    }

    public async Task<ApiResponse<dynamic>> AddAsync(DynamicFormViewModel viewModel)
    {
        try
        {
            viewModel.IpAddress = IpAddressHelper.GetClientIpAddress(_httpContext.HttpContext);
            var inserted = await _iFormValue.AddAsync(viewModel);
            if (!inserted)
                return ApiResponse<dynamic>.Fail("Submission failed!");

            var business = await _iGenericBusinessInfo.GetByIdAsync(viewModel.BusinessId);
            if (business is null)
                return ApiResponse<dynamic>.Fail("Business not found");

            #region - send email notification to business owner -
            var subject = "Submission Receive Notification";
            var body = await BuildHtmlBodyAsync(viewModel.Inputs);

            if (!string.IsNullOrWhiteSpace(body))
            {
                // Queue background email task
                _iTaskQueue.QueueBackgroundWorkItem(async sp =>
                {
                    var emailService = sp.GetRequiredService<IEmailService>();
                    try
                    {
                        await emailService.SendMailAsync(business.Email, subject, body);
                    }
                    catch (Exception ex)
                    {
                        // Log failure
                        Log.Error(ex, "Failed to send email to {Email}", business.Email);
                    }
                });
            }
            #endregion

            return ApiResponse<dynamic>.Success(true, "Submitted successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error adding DynamicForm: {@ViewModel}", JsonSerializer.Serialize(viewModel));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }

    #region - form settings -
    public async Task<ApiResponse<DynamicFormViewModel>> GetDynamicFormAsync(int? businessId)
    {
        try
        {
            var result = await _iFormValue.GetDynamicFormAsync(businessId);
            return ApiResponse<DynamicFormViewModel>.Success(result, "Form values retrieved successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error retrieving dynamic form for BusinessId: {BusinessId}", businessId);
            return ApiResponse<DynamicFormViewModel>.Fail("Something went wrong!");
        }
    }
    public async Task<ApiResponse<dynamic>> UpdateFormSettingsAsync(UpdateFormSettingsRequest request)
    {
        try
        {
            request.Username = CurrentUser;
            await _iFormValue.UpdateFormSettingsAsync(request);
            return ApiResponse<dynamic>.Success(true, "Form updated successfully!");
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Error updating DynamicForm: {@ViewModel}", JsonSerializer.Serialize(request));
            return ApiResponse<dynamic>.Fail("Something went wrong!");
        }
    }
    #endregion

    #region - email body generation -
    private async Task<string> BuildHtmlBodyAsync(List<DynamicFormDto>? formData)
    {
        if (formData == null || formData.Count == 0) return "";

        var sb = new System.Text.StringBuilder();

        sb.Append(@"
<html>
<head>
  <style>
    body {
      font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
      background-color: #f4f6f8;
      padding: 30px;
    }
    .container {
      max-width: 650px;
      margin: auto;
      background: #ffffff;
      border-radius: 10px;
      padding: 25px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
    }
    h2 {
      text-align: center;
      color: #2c3e50;
      margin-bottom: 20px;
    }
    table {
      width: 100%;
      border-collapse: collapse;
    }
    tr {
      border-bottom: 1px solid #e0e0e0;
    }
    td {
      padding: 12px 10px;
      vertical-align: top;
    }
    .label {
      font-weight: 600;
      color: #34495e;
      width: 35%;
      white-space: nowrap;
    }
    .value {
      color: #555555;
      width: 65%;
    }
    tr:last-child {
      border-bottom: none;
    }
  </style>
</head>
<body>
  <div class='container'>
    <h2>Form Submission</h2>
    <table>
");

        foreach (var item in formData)
        {
            if (item is null) continue;
            if (string.IsNullOrWhiteSpace(item.Value)) continue;
            if (item.FormDetailId is null) continue;

            var detail = await _iGenericFromDetail.GetByIdAsync(item.FormDetailId.Value);
            if (detail is null) continue;

            sb.AppendFormat(@"
      <tr>
        <td class='label'>{0}</td>
        <td class='value'>{1}</td>
      </tr>",
            System.Net.WebUtility.HtmlEncode(detail.Name),
            System.Net.WebUtility.HtmlEncode(item.Value));
        }

        sb.Append(@"
    </table>
  </div>
</body>
</html>");

        return sb.ToString();
    }

    #endregion
}

