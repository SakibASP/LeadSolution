namespace Core.ViewModels.Dto.Menu;

public class MenuSelectionDto
{
    public int? MenuId { get; set; }
    public string? MenuName { get; set; }
    public bool IsSelected { get; set; }
    public int? MenuParentId { get; set; }
    public int? Count { get; set; }
}
