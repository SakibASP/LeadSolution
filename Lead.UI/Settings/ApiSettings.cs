namespace Lead.UI.Settings;

public class ApiSettings
{
    public string BaseUrl { get; set; } = default!;
    public Controllers Controllers { get; set; } = default!;
    public Endpoints Endpoints { get; set; } = default!;
}

public class Controllers
{
    public string Auth { get; set; } = default!;
    public string Roles { get; set; } = default!;
    public string MaintainUser { get; set; } = default!;
    public string Menu { get; set; } = default!;
    public string AdminRights { get; set; } = default!;
    public string DataTypes { get; set; } = default!;
    public string FormDetails { get; set; } = default!;
    public string FormValues { get; set; } = default!;
    public string BusinessInfo { get; set; } = default!;
    public string Utility { get; set; } = default!;
}

public class Endpoints
{
    public CommonEndpoints CommonEndPoints { get; set; } = default!;
    public AuthEndpoints Auth { get; set; } = default!;
    public RoleEndpoints Roles { get; set; } = default!;
    public MaintainUserEndpoints MaintainUser { get; set; } = default!;
    public MenuEndpoints Menu { get; set; } = default!;
    public AdminRightsEndpoints AdminRights { get; set; } = default!;
    public FormValuesEndpoints FormValues { get; set; } = default!;
    public BusinessInfoEndpoints BusinessInfo { get; set; } = default!;
    public UtilityEndpoints Utility { get; set; } = default!;
}

public class CommonEndpoints
{
    public string GetAll { get; set; } = default!;
    public string GetById { get; set; } = default!;
    public string Add { get; set; } = default!;
    public string Update { get; set; } = default!;
    public string Remove { get; set; } = default!;
}

public class AuthEndpoints
{
    public string Login { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public string Register { get; set; } = default!;
    public string GetApiKey { get; set; } = default!;
    public string GenerateApiKey { get; set; } = default!;
}

public class RoleEndpoints
{
    public string GetRoles { get; set; } = default!;
    public string GetRoleById { get; set; } = default!;
    public string AddRole { get; set; } = default!;
    public string UpdateRole { get; set; } = default!;
    public string DeleteRole { get; set; } = default!;
}

public class MaintainUserEndpoints
{
    public string GetUsers { get; set; } = default!;
    public string AssignRole { get; set; } = default!;
    public string GetUserRoles { get; set; } = default!;
    public string GetUserProfile { get; set; } = default!;
    public string UpdateProfile { get; set; } = default!;
    public string ChangePassword { get; set; } = default!;
}

public class MenuEndpoints
{
    public string GetByUserId { get; set; } = default!;

}
public class AdminRightsEndpoints
{
    public string GetAllMenu { get; set; } = default!;
    public string GetRoleWiseMenu { get; set; } = default!;
    public string CreateRoleWiseMenu { get; set; } = default!;
    public string UpdateRoleWiseMenu { get; set; } = default!;
}

public class FormValuesEndpoints
{
    public string GetDynamicForm { get; set; } = default!;
    public string UpdateFormSettings { get; set; } = default!;
}

public class BusinessInfoEndpoints
{
    public string GetServiceType { get; set; } = default!;
}

public class UtilityEndpoints
{
    public string GetDropdown { get; set; } = default!;
    public string GetUserDropdown { get; set; } = default!;
    public string GetSystemLogs { get; set; } = default!;
    public string GetSystemLogById { get; set; } = default!;
    public string GetApiLogs { get; set; } = default!;
    public string GetApiLogById { get; set; } = default!;
}