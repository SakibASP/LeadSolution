namespace Core.ViewModels.Dto.Menu;

public class MenuItemViewModel
{
    public string? RoleId { get; set; }
    public IList<MenuSelectionDto> MenuSelectionDto { get; set; } = [];
}
