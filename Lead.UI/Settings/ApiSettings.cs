namespace Lead.UI.Settings;

public class ApiSettings
{
    public string BaseUrl { get; set; }
    public Controllers Controllers { get; set; }
    public Endpoints Endpoints { get; set; }
}

public class Controllers
{
    public string Auth { get; set; }
    public string Roles { get; set; }
    public string MaintainUser { get; set; }
    public string Menu { get; set; }
    public string AdminRights { get; set; }
    public string DataTypes { get; set; }
    public string FormDetails { get; set; }
    public string FormValues { get; set; }
    public string BusinessInfo { get; set; }
    public string Utility { get; set; }
}

public class Endpoints
{
    public CommonEndpoints CommonEndPoints { get; set; }
    public AuthEndpoints Auth { get; set; }
    public RoleEndpoints Roles { get; set; }
    public MaintainUserEndpoints MaintainUser { get; set; }
    public MenuEndpoints Menu { get; set; }
    public AdminRightsEndpoints AdminRights { get; set; }
    public FormValuesEndpoints FormValues { get; set; }
    public BusinessInfoEndpoints BusinessInfo { get; set; }
    public UtilityEndpoints Utility { get; set; }
}

public class CommonEndpoints
{
    public string GetAll { get; set; }
    public string GetById { get; set; }
    public string Add { get; set; }
    public string Update { get; set; }
    public string Remove { get; set; }
}

public class AuthEndpoints
{
    public string Login { get; set; }
    public string RefreshToken { get; set; }
    public string Register { get; set; }
}

public class RoleEndpoints
{
    public string GetRoles { get; set; }
    public string GetRoleById { get; set; }
    public string AddRole { get; set; }
    public string UpdateRole { get; set; }
    public string DeleteRole { get; set; }
}

public class MaintainUserEndpoints
{
    public string GetUsers { get; set; }
    public string AssignRole { get; set; }
    public string GetUserRoles { get; set; }
}

public class MenuEndpoints
{
    public string GetByUserId { get; set; }

}
public class AdminRightsEndpoints
{
    public string GetAllMenu { get; set; }
    public string GetRoleWiseMenu { get; set; }
    public string CreateRoleWiseMenu { get; set; }
    public string UpdateRoleWiseMenu { get; set; }
}

public class FormValuesEndpoints
{
    public string GetDynamicForm { get; set; }
}

public class BusinessInfoEndpoints
{
    public string GetServiceType { get; set; }
}

public class UtilityEndpoints
{
    public string GetDropdown { get; set; }
    public string GetUserDropdown { get; set; }
}