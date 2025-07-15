namespace Core.ViewModels.Dto.Menu;

public class DynamicMenuItemDto
{
    public int? MID { get; set; }
    public string? MenuName { get; set; }
    public string? MenuURL { get; set; }
    public int? MenuParentID { get; set; }
    public string? FaIcon { get; set; }
}
