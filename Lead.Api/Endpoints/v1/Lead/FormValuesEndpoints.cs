using Application.Interfaces.Lead;
using Core.ViewModels.Dto.Lead;
using Core.ViewModels.Request.Lead;
using Lead.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Endpoints.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 26 July 2025
/// </summary>

public static class FormValuesEndpoints
{
    public static void MapFormValuesEndpoints(this IEndpointRouteBuilder app)
    {
        // /api/v1/formvalues
        var group = app.MapGroup("api/v1/formvalues")
                       .WithTags("FormValues")
                       .RequireAuthorization();

        // GET: api/v1/formvalues/get-all
        group.MapGet("get-all", async ([AsParameters] GetFormValueRequest request,
                                       IFormValueService service) =>
        {
            var result = await service.GetMessagesByBusinessAsync(request);
            return Results.Ok(result);
        });

        // GET: api/v1/formvalues/get-message-details-by-id?masterId=1
        group.MapGet("get-message-details-by-id", async ([FromQuery] int masterId,
                                                         IFormValueService service) =>
        {
            var result = await service.GetMessagesDetailsByMasterIdAsync(masterId);
            return Results.Ok(result);
        });

        // GET: api/v1/formvalues/get-dynamic-form?businessId=10
        group.MapGet("get-dynamic-form", async ([FromQuery] int? businessId,
                                                IFormValueService service) =>
        {
            var result = await service.GetDynamicFormAsync(businessId);
            return Results.Ok(result);
        });

        // POST: api/v1/formvalues/update-form-settings
        group.MapPost("update-form-settings", async ([FromBody] UpdateFormSettingsRequest request,
                                                     IFormValueService service) =>
        {
            var result = await service.UpdateFormSettingsAsync(request);
            return Results.Ok(result);
        });

        // POST: api/v1/formvalues/add
        group.MapPost("add", async ([FromBody] DynamicFormViewModel formValues,
                                    IFormValueService service) =>
        {
            var result = await service.AddAsync(formValues);
            return Results.Ok(result);
        })
        .AllowAnonymous()                    // replaces [AllowAnonymous]
        .AddEndpointFilter<ApiKeyAuthFilter>(); // replaces [ApiKeyAuth]
    }
}
