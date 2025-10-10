using Application.Interfaces.Menu;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Response;
using Serilog;
using System.Security.Claims;

namespace Lead.Api.Endpoints.v1.Menu;

public static class MenuEndpoints
{
    public static void MapMenuEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/menu")
                       .WithTags("Menu")
                       .RequireAuthorization(); // Equivalent to [Authorize]

        group.MapGet("get-menu-by-user", async (IMenuService menuService, IHttpContextAccessor httpContext) =>
        {
            var httpContextValue = httpContext.HttpContext;
            var currentUser = httpContextValue?.User?.Identity?.Name ?? "N/A";
            var requestPath = httpContextValue?.Request.Path.Value ?? "N/A";

            try
            {
                var user = httpContextValue?.User;
                var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

                var menu = await menuService.GetAllMenuAsync(userId);
                return Results.Ok(menu);
            }
            catch (Exception ex)
            {
                Log
                    .ForContext("UserName", currentUser)
                    .ForContext("Path", requestPath)
                    .Error(ex, "Error getting menu");

                return Results.Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
            }

        })
        .WithName("GetMenuByUser")
        .WithSummary("Get menu list for the logged-in user");
    }
}
