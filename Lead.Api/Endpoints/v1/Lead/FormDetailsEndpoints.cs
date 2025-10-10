using Application.Interfaces.Lead;
using Core.Models.Lead;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Endpoints.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 26 July 2025
/// </summary>
public static class FormDetailsEndpoints
{
    public static void MapFormDetailsEndpoints(this IEndpointRouteBuilder app)
    {
        // Group routes under api/v1/formdetails with authorization
        var group = app.MapGroup("api/v1/formdetails")
                       .WithTags("FormDetails")
                       .RequireAuthorization();

        // GET: api/v1/formdetails/get-all?parameter=...
        group.MapGet("get-all", async (IFormDetailService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        });

        // GET: api/v1/formdetails/get-by-id?id=1
        group.MapGet("get-by-id", async ([FromQuery] int id, IFormDetailService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return Results.Ok(result);
        });

        // POST: api/v1/formdetails/add
        group.MapPost("add", async ([FromBody] FormDetails formDetails, IFormDetailService service) =>
        {
            var result = await service.AddAsync(formDetails);
            return Results.Ok(result);
        });

        // POST: api/v1/formdetails/update
        group.MapPost("update", async ([FromBody] FormDetails formDetails, IFormDetailService service) =>
        {
            var result = await service.UpdateAsync(formDetails);
            return Results.Ok(result);
        });

        // POST: api/v1/formdetails/remove
        group.MapPost("remove", async ([FromBody] int id, IFormDetailService service) =>
        {
            var result = await service.RemoveAsync(id);
            return Results.Ok(result);
        });
    }
}
