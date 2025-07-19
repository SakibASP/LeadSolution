using Core.ViewModels.Dto.Menu;

namespace Core.ViewModels.Request.Menu;

public class UpdateAdminRightsRequest
{
    public IList<MenuSelectionDto>? MenuSelections { get; set; }
    public string? RoleId { get; set; }
}
