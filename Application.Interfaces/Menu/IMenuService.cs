using Core.ViewModels.Dto.Menu;
using Core.ViewModels.Response;

namespace Application.Interfaces.Menu
{
    public interface IMenuService
    {
        Task<ApiResponse<IList<DynamicMenuItemDto>>> GetAllMenuAsync(string? userId);
    }
}
