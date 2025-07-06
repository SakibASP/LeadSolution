namespace Core.ViewModels.Dto.Auth
{
    public class AuthResponseDto
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}
