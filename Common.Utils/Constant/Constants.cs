namespace Common.Utils.Constant;

public static class Constants
{
    //Session keys
    public const string AuthResponseDto = "AuthResponseDto";
    public const string Menu = "Menu"; 

    //Redirections
    public const string RedirectToLogin = "/Auth/Login";

    //Roles
    public const string AdminAuthRoles = "Admin, Super Admin"; 
    public const string Admin = "Admin"; 
    public const string SuperAdmin = "Super Admin"; 


    //TempMessage keys
    public const string Error = "Error";
    public const string Success = "Success";
}

public static class Sp
{
    public const string usp_get_dropdownList = "exec usp_get_dropdownList";
}