namespace Core.ViewModels.Dto.Auth.Roles;

public class RoleDto
{
    public virtual string Id { get; set; } = default!;
    public virtual string? Name { get; set; }
    public virtual string? NormalizedName { get; set; }
    public virtual string? ConcurrencyStamp { get; set; }
}
