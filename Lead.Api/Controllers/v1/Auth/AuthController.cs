using Application.Interfaces.Auth;
using Application.Interfaces.Menu;
using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth;
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
/// 19 July 2025
/// </summary>

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
    ITokenService tokenService, IOptions<JwtOptions> jwtOptions, IServiceTypeService serviceType,
    IAdminRightsService adminRights) : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly ITokenService _iTokenService = tokenService;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly IServiceTypeService _iServiceType = serviceType;
    private readonly IAdminRightsService _iAdminRights = adminRights;

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
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
        
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
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
                Expires = TimeHelper.GetCurrentBangladeshTime().AddDays(_jwtOptions.RefreshTokenValidityInDays)
            };

            var encryptedToken = EncryptionHelper.Encrypt(JsonSerializer.Serialize(tokenDto));
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", encryptedToken);

            // Wrap auth response inside ApiResponse<AuthResponseDto>
            var response = new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                UserName = user.UserName,
                Expiration = TimeHelper.GetCurrentBangladeshTime().AddMinutes(_jwtOptions.TokenValidityInMinutes)
            };

            return Ok(ApiResponse<AuthResponseDto>.Success(response, "Login successful!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
    }


    [HttpPost]
    [Route("refresh-token")]
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
            if (tokenData.Expires >= TimeHelper.GetCurrentBangladeshTime())
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider: "Default", tokenName: "RefreshToken");
                return Ok(ApiResponse<AuthResponseDto>.Fail("Refresh token expired!"));
            }

            var newAccessToken = await _iTokenService.GenerateJwtToken(user);
            var response = new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = tokenData.RefreshToken,
                Expiration = TimeHelper.GetCurrentBangladeshTime().AddMinutes(_jwtOptions.TokenValidityInMinutes)
            };
            return Ok(ApiResponse<AuthResponseDto>.Success(response, "Token refreshed successful!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return 
                Ok(ApiResponse<AuthResponseDto>.Fail("Invalid user name"));
        await _userManager.RemoveAuthenticationTokenAsync(user,loginProvider: "Default",tokenName: "RefreshToken");

        return Ok(new { Status = "Success", Message = "Token revoked" });
    }

    [Authorize]
    [HttpPost]
    [Route("revoke-all")]
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

    #region Service Type

    [HttpGet]
    [Route("service-type")]
    public async Task<IActionResult> ServiceType()
    {
        try
        {
            var types = await _iServiceType.GetAspNetServiceTypesAsync();
            return Ok(ApiResponse<IList<AspNetServiceTypes>>.Success(types, "Token refreshed successful!"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
        }
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
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
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
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
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
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
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
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
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
            Log.Error(ex, MessageHelper<string>.GenerateErrorMsg(HttpContext.Request.Path, null, User.Identity?.Name));
            return Ok(ApiResponse<string>.Fail("Something went wrong!"));
        }
    }
    #endregion

    [Authorize(Roles = Constants.AdminAuthRoles)]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return NotFound("User not found.");

        if (!await _roleManager.RoleExistsAsync(request.Role))
            return NotFound("Role not found.");

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        return result.Succeeded ? Ok("Role assigned.") : BadRequest(result.Errors);
    }

}
