namespace Lead.UI.Settings;

public class ApiSettings
{
    public required string BaseUrl { get; set; }
    public required ControllerNames Versions { get; set; }
    public required ControllerNames ControllerNames { get; set; }
    public required Endpoints Endpoints { get; set; }
}

public class ControllerNames
{
    public required string Auth { get; set; }
    public required string Menu { get; set; }
    public required string DataTypes { get; set; }
    public required string FormDetails { get; set; }
    public required string FormValues { get; set; }
    public required string BusinessInfo { get; set; }
}


public class Endpoints
{
    public required CommonEndPoints CommonEndPoints { get; set; }
    public required CAuth Auth { get; set; }
    public required CMenu Menu { get; set; }
    public required CFormValues FormValues { get; set; }
    public required CBusinessInfo BusinessInfo { get; set; }
}

public class CommonEndPoints
{
    public required string GetAll { get; set; }
    public required string GetById { get; set; }
    public required string Add { get; set; }
    public required string Update { get; set; }
    public required string Remove { get; set; }
}

public class CAuth
{
    //Authentication and User management
    public required string Login { get; set; }
    public required string RefreshToken { get; set; }
    public required string Register { get; set; }
    public required string GetUsers { get; set; }
    public required string GetUserById { get; set; }
    public required string AssignRole { get; set; }

    //Roles management
    public required string GetUserRoles { get; set; }
    public required string GetRoles { get; set; }
    public required string GetRoleById { get; set; }
    public required string GetRolesByUserId { get; set; }
    public required string AddRole { get; set; }
    public required string UpdateRole { get; set; }
    public required string DeleteRole { get; set; }

}

public class CMenu
{
    //Menu management
    public required string GetByUserId { get; set; }

    //User rights management
    public required string GetAllMenu { get; set; }
    public required string GetRoleWiseMenu { get; set; }
    public required string CreateRoleWiseMenu { get; set; }
    public required string UpdateRoleWiseMenu { get; set; }
}

public class CFormValues
{
    public required string GetDynamicForm { get; set; }
}

public class CBusinessInfo
{
    public required string GetServiceType { get; set; }
}