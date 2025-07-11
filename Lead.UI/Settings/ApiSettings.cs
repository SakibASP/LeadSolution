namespace Lead.UI.Settings
{
    public class ApiSettings
    {
        public required string BaseUrl { get; set; }
        public required Versions Versions { get; set; }
        public required Endpoints Endpoints { get; set; }
    }

    public class Versions
    {
        public required string Auth { get; set; }
        public required string Menu { get; set; }
    }

    public class Endpoints
    {
        public required Auth Auth { get; set; }
        public required Menu Menu { get; set; }
    }

    public class  Auth
    {
        public required string Login { get; set; }
        public required string RefreshToken { get; set; }
        public required string Register { get; set; }
        public required string GetUsers { get; set; }
        public required string GetUserById { get; set; }
        public required string GetRoles { get; set; }
        public required string GetRoleByUserId { get; set; }
        public required string AddRole { get; set; }
        public required string ServiceType { get; set; }

    }

    public class Menu
    {
        public required string GetByUserId { get; set; }
    }
}
