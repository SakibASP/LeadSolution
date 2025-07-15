namespace Core.Models.Auth;

public class AspNetServiceTypes
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; } = true;
}
