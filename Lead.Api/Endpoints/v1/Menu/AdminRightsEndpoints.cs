using Application.Interfaces.Menu;
using Common.Utils.Constant;
using Core.Models.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Request.Menu;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Lead.Api.Endpoints.v1.Menu;

public static class AdminRightsEndpoints
{
    public static void MapAdminRightsEndpoints(this IEndpointRouteBuilder app)
    {
        var roles = Constants.AdminAuthRoles.Split(',').Select(r => r.Trim()).ToArray();
        var group = app.MapGroup("api/v1/adminrights")
                       .WithTags("AdminRights")
                       .RequireAuthorization(policy =>
                           policy.RequireRole(roles));

        // GET: api/v1/adminrights/get-all-menu
        group.MapGet("get-all-menu", async (IAdminRightsService service, IHttpContextAccessor httpContext) =>
        {
            try
            {
                var user = httpContext.HttpContext?.User;
                var currentUser = user?.Identity?.Name ?? "N/A";
                var path = httpContext.HttpContext?.Request.Path.Value ?? "N/A";
                var isSuperAdmin = user?.IsInRole(Constants.SuperAdmin) ?? false;

                var menu = await service.GetMenuMasterAsync(isSuperAdmin);
                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                Log.ForContext("UserName", httpContext.HttpContext?.User?.Identity?.Name ?? "N/A")
                   .ForContext("Path", httpContext.HttpContext?.Request.Path.Value ?? "N/A")
                   .Error(ex, "Error getting all menu");

                return Results.Ok(ApiResponse<IList<MenuItem>>.Fail("Something went wrong!"));
            }
        });

        // GET: api/v1/adminrights/get-role-wise-menu?roleId=123
        group.MapGet("get-role-wise-menu", async ([FromQuery] string roleId,
                                                  IAdminRightsService service,
                                                  IHttpContextAccessor httpContext) =>
        {
            try
            {
                var menu = await service.GetRoleWiseSelectedPagesAsync(roleId);
                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                Log.ForContext("UserName", httpContext.HttpContext?.User?.Identity?.Name ?? "N/A")
                   .ForContext("Path", httpContext.HttpContext?.Request.Path.Value ?? "N/A")
                   .Error(ex, "Error getting role-wise menu for RoleId: {RoleId}", roleId);

                return Results.Ok(ApiResponse<IList<MenuItemViewModel>>.Fail("Something went wrong!"));
            }
        });

        // POST: api/v1/adminrights/create-role-wise-menu
        group.MapPost("create-role-wise-menu", async ([FromBody] MenuItemViewModel menuItem,
                                                      IAdminRightsService service,
                                                      IHttpContextAccessor httpContext) =>
        {
            try
            {
                var menu = await service.CreateAsync(menuItem);
                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                Log.ForContext("UserName", httpContext.HttpContext?.User?.Identity?.Name ?? "N/A")
                   .ForContext("Path", httpContext.HttpContext?.Request.Path.Value ?? "N/A")
                   .Error(ex, "Error creating role-wise menu: {@MenuItem}", menuItem);

                return Results.Ok(ApiResponse<string>.Fail("Something went wrong!"));
            }
        });

        // POST: api/v1/adminrights/update-role-wise-menu
        group.MapPost("update-role-wise-menu", async ([FromBody] UpdateAdminRightsRequest updateRequest,
                                                      IAdminRightsService service,
                                                      IHttpContextAccessor httpContext) =>
        {
            try
            {
                var menu = await service.UpdateRecordsAsync(updateRequest);
                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                Log.ForContext("UserName", httpContext.HttpContext?.User?.Identity?.Name ?? "N/A")
                   .ForContext("Path", httpContext.HttpContext?.Request.Path.Value ?? "N/A")
                   .Error(ex, "Error updating role-wise menu: {@UpdateRequest}", updateRequest);

                return Results.Ok(ApiResponse<string>.Fail("Something went wrong!"));
            }
        });
    }
}
