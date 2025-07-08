using Core.ViewModels.Dto.Menu;

namespace Infrustructure.Interfaces.Menu
{
    public interface IMenuRepo
    {
        Task<IList<DynamicMenuItemDto>> GetAllMenuAsync(string? userId);
    }
}
