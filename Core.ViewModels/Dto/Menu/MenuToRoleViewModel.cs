namespace Core.ViewModels.Dto.Menu;

public class MenuToRoleViewModel
{
    public List<MenuSelectionDto> MenuSelections { set; get; } = [];
    public string? RoleId { set; get; }
    public List<int>? MenuParentIds { get; internal set; }
}
