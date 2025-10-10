using Application.Interfaces.Lead;
using Core.Models.Auth;
using Core.ViewModels.Dto.Lead;

namespace Lead.Api.Endpoints.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 27 July 2025
/// </summary>
public static class BusinessInfoEndpoints
{
    public static void MapBusinessInfoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/businessinfo")
                       .WithTags("BusinessInfo")
                       .RequireAuthorization();

        group.MapGet("get-all", async (IBusinessInfoService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        });

        group.MapGet("get-by-id", async (int id, IBusinessInfoService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return Results.Ok(result);
        });

        group.MapPost("add", async (AspNetBusinessInfoDto businessInfo, IBusinessInfoService service) =>
        {
            var result = await service.AddAsync(businessInfo);
            return Results.Ok(result);
        });

        group.MapPost("update", async (AspNetBusinessInfo businessInfo, IBusinessInfoService service) =>
        {
            var result = await service.UpdateAsync(businessInfo);
            return Results.Ok(result);
        });

        group.MapPost("remove", async (int id, IBusinessInfoService service) =>
        {
            var result = await service.RemoveAsync(id);
            return Results.Ok(result);
        });
    }
}
