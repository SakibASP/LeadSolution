using Application.Interfaces.Lead;
using Core.Models.Lead;
using Microsoft.AspNetCore.Mvc;

namespace Lead.Api.Endpoints.v1.Lead;

/// <summary>
/// Md. Sakibur Rahman
/// 25 July 2025
/// </summary>
public static class DataTypesEndpoints
{
    public static void MapDataTypesEndpoints(this IEndpointRouteBuilder app)
    {
        // Group all endpoints with version + controller name
        var group = app.MapGroup("api/v1/datatypes")
                       .WithTags("DataTypes")
                       .RequireAuthorization();

        // GET: api/v1/datatypes/get-all?parameter=...
        group.MapGet("get-all", async ( IDataTypeService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        });

        // GET: api/v1/datatypes/get-by-id?id=1
        group.MapGet("get-by-id", async ([FromQuery] int id, IDataTypeService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return Results.Ok(result);
        });

        // POST: api/v1/datatypes/add
        group.MapPost("add", async ([FromBody] DataTypes dataTypes, IDataTypeService service) =>
        {
            var result = await service.AddAsync(dataTypes);
            return Results.Ok(result);
        });

        // POST: api/v1/datatypes/update
        group.MapPost("update", async ([FromBody] DataTypes dataTypes, IDataTypeService service) =>
        {
            var result = await service.UpdateAsync(dataTypes);
            return Results.Ok(result);
        });

        // POST: api/v1/datatypes/remove
        group.MapPost("remove", async ([FromBody] int id, IDataTypeService service) =>
        {
            var result = await service.RemoveAsync(id);
            return Results.Ok(result);
        });
    }
}
