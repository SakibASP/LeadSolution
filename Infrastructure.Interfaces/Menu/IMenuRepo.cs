using Core.ViewModels.Dto.Menu;

namespace Infrastructure.Interfaces.Menu;

public interface IMenuRepo
{
    /// <summary>
    /// Retrieves all dynamic menu items available for the specified user.
    /// <param name="userId">The unique identifier of the user for whom to fetch menu items. If null, returns menu items for all users or default menu.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of DynamicMenuItemDto objects.</returns>
    /// </summary>
    Task<IList<DynamicMenuItemDto>> GetAllMenuAsync(string? userId);
}
