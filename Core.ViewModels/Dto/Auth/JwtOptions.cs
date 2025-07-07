namespace Core.ViewModels.Dto.Auth
{
    public class JwtOptions
    {
        public required string ValidIssuer { get; set; }
        public required string ValidAudience { get; set; }
        public required string Secret { get; set; }
        public required int TokenValidityInMinutes { get; set; }
        public required int RefreshTokenValidityInDays { get; set; }
    }

}
