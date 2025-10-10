using Application.Interfaces.Common;
using Common.Utils.Constant;
using Core.ViewModels.Request.Common;
using Lead.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Endpoints.v1.Common;

/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>
public static class UtilityEndpoints
{
    public static void MapUtilityEndpoints(this IEndpointRouteBuilder app)
    {
        // Group with default authorization
        var group = app.MapGroup("api/v1/utility")
                       .WithTags("Utility")
                       .RequireAuthorization();

        // GET: api/v1/utility/get-dropdown
        group.MapGet("get-dropdown", async ([AsParameters] DropdownRequest request, IUtilityService service) =>
        {
            var result = await service.GetDropdownListAsync(request);
            return Results.Ok(result);
        });

        // GET: api/v1/utility/get-user-dropdown
        group.MapGet("get-user-dropdown", async ([AsParameters] DropdownRequest request, IUtilityService service) =>
        {
            var result = await service.GetUserDropdownListAsync(request);
            return Results.Ok(result);
        });

        // GET: api/v1/utility/get-client-dropdown
        // AllowAnonymous + ApiKeyAuth
        group.MapGet("get-client-dropdown", async ([AsParameters] DropdownRequest request, IUtilityService service) =>
        {
            var result = await service.GetClientDropdownListAsync(request);
            return Results.Ok(result);
        })
        .AllowAnonymous()
        .AddEndpointFilter<ApiKeyAuthFilter>();

        // POST: api/v1/utility/update-country-data
        group.MapPost("update-country-data", async (IUtilityService service) =>
        {
            var result = await service.UpdateCountriesAsync();
            return Results.Ok(result);
        });

        // SuperAdmin-only endpoints
        #region - SuperAdmin Endpoints -
        var superAdminGroup = app.MapGroup("api/v1/utility")
                                 .WithTags("Utility")
                                 .RequireAuthorization(policy =>
                                     policy.RequireRole(Constants.SuperAdmin));

        superAdminGroup.MapGet("get-system-logs", async (IUtilityService service) =>
        {
            var result = await service.GetSystemLogsAsync();
            return Results.Ok(result);
        });

        superAdminGroup.MapGet("get-system-log-by-id", async ([FromQuery] int id, IUtilityService service) =>
        {
            var result = await service.GetSystemLogByIdAsync(id);
            return Results.Ok(result);
        });

        superAdminGroup.MapGet("get-api-logs", async (IUtilityService service) =>
        {
            var result = await service.GetApiLogsAsync();
            return Results.Ok(result);
        });

        superAdminGroup.MapGet("get-api-log-by-id", async ([FromQuery] int id, IUtilityService service) =>
        {
            var result = await service.GetApiLogByIdAsync(id);
            return Results.Ok(result);
        });
        #endregion
    }
}
