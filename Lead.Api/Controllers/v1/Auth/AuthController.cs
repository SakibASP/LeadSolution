using Application.Interfaces.Auth;
using Application.Interfaces.Menu;
using Common.Extentions;
using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Auth.Roles;
using Core.ViewModels.Request.Auth;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System.Data;
using System.Text.Json;

namespace Lead.Api.Controllers.v1.Auth;

/// <summary>
/// Md. Sakibur Rahman
/// 10 July 2025
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
    ITokenService tokenService, IOptions<JwtOptions> jwtOptions,
    IAdminRightsService adminRights) : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly ITokenService _iTokenService = tokenService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IAdminRightsService _iAdminRights = adminRights;
    private readonly DateTime _bdTime = DateTime.Now.ToBangladeshTime();

    #region Login/Register/RefreshToken and Revoke user

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join("; ", result.Errors.Select(e => e.Description));
                return Ok(ApiResponse<string>.Fail(errorMessages));
            }

            return Ok(ApiResponse<string>.Success("User created successfully"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
        
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Ok(ApiResponse<string>.Fail("Invalid email or password"));

            var token = await _iTokenService.GenerateJwtToken(user);
            var refreshToken = _iTokenService.GenerateRefreshToken();

            // Store refresh token with expiration as JSON
            var tokenDto = new TokenDto
            {
                RefreshToken = refreshToken,
                Expires = _bdTime.AddDays(_jwtOptions.RefreshTokenValidityInDays)
            };

            var encryptedToken = EncryptionHelper.Encrypt(JsonSerializer.Serialize(tokenDto));
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", encryptedToken);

            // Wrap auth response inside ApiResponse<AuthResponseDto>
            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                UserName = user.UserName,
                Expiration = _bdTime.AddMinutes(_jwtOptions.TokenValidityInMinutes)
            };

            return Ok(ApiResponse<AuthResponseDto>.Success(response, "Login successful!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto dto)
    {
        try
        {
            if (dto is null || string.IsNullOrEmpty(dto.RefreshToken) || string.IsNullOrEmpty(dto.AccessToken)) return BadRequest("Invalid client request!");

            // 1. Get claims principal from expired access token
            var principal = _iTokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal is null)
                return Ok(ApiResponse<AuthResponseDto>.Fail("Invalid access token or refresh token!"));

            var username = principal.Identity!.Name;
            if (string.IsNullOrEmpty(username))
                return Ok(ApiResponse<AuthResponseDto>.Fail("Invalid user!"));

            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                return Ok(ApiResponse<AuthResponseDto>.Fail("Invalid user!"));

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "RefreshToken");
            if (storedToken is null)
                return Ok(ApiResponse<AuthResponseDto>.Fail("No refresh token found for this user!"));

            var tokenData = JsonSerializer.Deserialize<TokenDto>(EncryptionHelper.Decrypt(storedToken));
            if (tokenData is null)
                return Ok(ApiResponse<AuthResponseDto>.Fail("Invalid refresh token data!"));

            if (tokenData.RefreshToken != dto.RefreshToken)
                return Ok(ApiResponse<AuthResponseDto>.Fail("Invalid refresh token!"));
            if (tokenData.Expires >= _bdTime)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider: "Default", tokenName: "RefreshToken");
                return Ok(ApiResponse<AuthResponseDto>.Fail("Refresh token expired!"));
            }

            var newAccessToken = await _iTokenService.GenerateJwtToken(user);
            var response = new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = tokenData.RefreshToken,
                Expiration = _bdTime.AddMinutes(_jwtOptions.TokenValidityInMinutes)
            };
            return Ok(ApiResponse<AuthResponseDto>.Success(response, "Token refreshed successful!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
    }

    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return 
                Ok(ApiResponse<AuthResponseDto>.Fail("Invalid user name"));
        await _userManager.RemoveAuthenticationTokenAsync(user,loginProvider: "Default",tokenName: "RefreshToken");

        return Ok(new { Status = "Success", Message = "Token revoked" });
    }

    [Authorize]
    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        var users = await _userManager.Users.ToListAsync();
        foreach (var user in users)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider: "Default", tokenName: "RefreshToken");
            await _userManager.UpdateAsync(user);
        }

        return Ok(new { Status = "Success", Message = "All tokens revoked" });
    }

    #endregion

    #region Role Management

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpGet("all-roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        try
        {
            var isSuperAdmin = User.IsInRole(Constants.SuperAdmin);
            var roles = await _iAdminRights.GetRoleListAsync(isSuperAdmin);
            return Ok(ApiResponse<IList<IdentityRole>>.Success(roles.Data, "Roles get successfully!")); // returns JSON list of roles
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<IList<IdentityRole>>.Fail("No role found!"));
        }
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpGet("get-role-by-Id")]
    public async Task<IActionResult> GetRoleById([FromQuery] string roleId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            return Ok(ApiResponse<IdentityRole>.Success(role, "Role get successfully!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<IdentityRole>.Fail("Something went wrong!"));
        }
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpPost("create-role")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return Ok(ApiResponse<string>.Fail("Role create failed!"));

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return Ok(ApiResponse<string>.Fail("Role already exists!"));

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            return result.Succeeded ? Ok(ApiResponse<string>.Success(null, "Role created successfully!")) : Ok(ApiResponse<string>.Fail("Role already exists!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<IList<IdentityRole>>.Fail("Something went wrong!"));
        }
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpPost("update-role")]
    public async Task<IActionResult> UpdateRole([FromBody] RoleDto role)
    {
        try
        {
            if (role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var _role = await _roleManager.FindByIdAsync(role.Id);
            if (_role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var setNameResult = await _roleManager.SetRoleNameAsync(_role, role.Name);
            if (!setNameResult.Succeeded)
                return Ok(ApiResponse<string>.Fail("Failed to set role name."));

            var updateResult = await _roleManager.UpdateAsync(_role);
            if (!updateResult.Succeeded)
                return Ok(ApiResponse<string>.Fail("Failed to update role."));

            return Ok(ApiResponse<string>.Success(null, "Role updated successfully!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpPost("delete-role")]
    public async Task<IActionResult> DeleteRole([FromBody] string roleId)
    {
        try
        {
            if (string.IsNullOrEmpty(roleId))
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role is null)
                return Ok(ApiResponse<string>.Fail("Role not found!"));

            await _roleManager.DeleteAsync(role);
            return Ok(ApiResponse<string>.Success(null, "Role removed successfully!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }

    #endregion

    #region Maintain User

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpGet("get-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var isSuperAdmin = User.IsInRole(Constants.SuperAdmin);
        var users = await _userManager.Users.ToListAsync();
        var userRolesViewModel = new List<UserRolesDto>();
        foreach (ApplicationUser user in users)
        {
            var thisViewModel = new UserRolesDto
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                NID = user.NID,
                Roles = await GetRoleNames(user)
            };
            userRolesViewModel.Add(thisViewModel);
        }
        if (!isSuperAdmin) userRolesViewModel = [.. userRolesViewModel.Where(x => !x.Roles!.Any(x => x.Equals(Constants.SuperAdmin, StringComparison.CurrentCultureIgnoreCase)))];
        return Ok(ApiResponse<IList<UserRolesDto>>.Success(userRolesViewModel,"Users get successfully"));
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpGet("get-user-roles")]
    public async Task<IActionResult> GetUserRoles([FromQuery] string userId)
    {
        var isSuperAdmin = User.IsInRole(Constants.SuperAdmin);
        var roles = await _iAdminRights.GetRoleListAsync(isSuperAdmin);
        var model = new List<ManageUserRoleDto>();
        var user = await _userManager.FindByIdAsync(userId);
        if (roles.Data is not null && user is not null)
        {
            foreach (var role in roles.Data)
            {
                var userRolesViewModel = new ManageUserRoleDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name!))
                    userRolesViewModel.Selected = true;
                else
                    userRolesViewModel.Selected = false;

                model.Add(userRolesViewModel);
            }
        }
        return Ok(ApiResponse<IList<ManageUserRoleDto>>.Success(model, "User roles get successfully"));
    }

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] UpdateUserRoleRequest request)
    {
        if (string.IsNullOrEmpty(request?.UserId)) return Ok(ApiResponse<string>.Fail("No user found!"));

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null || request.ManageUsers is null) return Ok(ApiResponse<string>.Fail("No user found!"));

        var roles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.RemoveFromRolesAsync(user, roles);

        if (!result.Succeeded) return Ok(ApiResponse<string>.Fail("Something went wrong!"));

        result = await _userManager.AddToRolesAsync(user, request.ManageUsers.Where(x => x.Selected).Select(y => y.RoleName)!);

        if (!result.Succeeded) return Ok(ApiResponse<string>.Fail("Something went wrong!"));

        return Ok(ApiResponse<string>.Success(null, "Role assigned successfully"));
    }

    #endregion

    private async Task<List<string>> GetRoleNames(ApplicationUser user)
    {
        return [.. await _userManager.GetRolesAsync(user)];
    }
}
