using Application.Interfaces.Menu;
using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Response;
using Infrustructure.Interfaces.Menu;

namespace Application.Services.Menu;

public class MenuService(IMenuRepo menu) : IMenuService
{
    private readonly IMenuRepo _iMenu = menu;
    public async Task<ApiResponse<IList<DynamicMenuItemDto>>> GetAllMenuAsync(string? userId)
    {
        var data = await _iMenu.GetAllMenuAsync(userId);
        if (data == null || !data.Any()) return ApiResponse<IList<DynamicMenuItemDto>>.Fail("No menu items found.");
        return ApiResponse<IList<DynamicMenuItemDto>>.Success(data);
    }
}
