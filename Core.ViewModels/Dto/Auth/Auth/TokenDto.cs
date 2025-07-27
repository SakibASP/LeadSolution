namespace Core.ViewModels.Dto.Auth.Auth;

public class TokenDto
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? Expires { get; set; }
}
