namespace Common.Utils.Constant;

public static class Constants
{
    //Session keys
    public const string AuthResponseDto = "AuthResponseDto";
    public const string Menu = "Menu"; 
    public const string UserInfo = "UserInfo"; 

    //Redirections
    public const string RedirectToLogin = "/Auth/Login";
    public const string RedirectToBusinessCreate = "/BusinessInfo/Create";

    //Roles
    public const string AdminAuthRoles = "Admin, Super Admin"; 
    public const string Admin = "Admin"; 
    public const string SuperAdmin = "Super Admin"; 
    public const string Client = "Client"; 
    public const string Moderator = "Moderator"; 
    public const string IT = "IT"; 


    //TempMessage keys
    public const string Error = "Error";
    public const string Success = "Success";
}

public static class Sp
{
    public const string usp_GetDropdownList = "dbo.usp_GetDropdownList";
    public const string usp_GetBusinessSupportedForms = "dbo.usp_GetBusinessSupportedForms";
    public const string usp_InsertUpdateBusinessSupportedForms = "dbo.usp_InsertUpdateBusinessSupportedForms";
    public const string usp_GetDynamicPivotedFormValues = "dbo.usp_GetDynamicPivotedFormValues";
}