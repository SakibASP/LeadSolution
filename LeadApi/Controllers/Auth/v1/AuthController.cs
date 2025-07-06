using Interfaces.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models.Auth;
using System.Text.Json;
using Utils.Helper;
using ViewModels.Auth;

namespace LeadApi.Controllers.Auth.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IOptions<JwtOptions> jwtOptions) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, PhoneNumber = dto.PhoneNumber };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized();

            var token = await _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Store refresh token in the database
            TokenDto tokenDto = new()
            {
                RefreshToken = refreshToken,
                Expires = DateTime.Now.AddDays(_jwtOptions.RefreshTokenValidityInDays)
            };
            var jsonRefreshToken = JsonSerializer.Serialize(tokenDto);
            await _userManager.SetAuthenticationTokenAsync(user, "Default", "RefreshToken", jsonRefreshToken);

            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                RefreshToken = refreshToken,
                Expiration = DateTime.Now.AddMinutes(_jwtOptions.TokenValidityInMinutes),
                Message = "Login successful"
            });
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto dto)
        {
            if (dto is null || string.IsNullOrEmpty(dto.RefreshToken) || string.IsNullOrEmpty(dto.AccessToken)) return BadRequest("Invalid client request");

            // 1. Get claims principal from expired access token
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal is null) return BadRequest("Invalid access token or refresh token");

            var username = principal.Identity.Name;
            if (string.IsNullOrEmpty(username)) return BadRequest("Invalid user");

            var user = await _userManager.FindByNameAsync(username);
            if (user is null) return BadRequest("Invalid user");

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "Default", "RefreshToken");
            if (storedToken is null) return Unauthorized("No refresh token found for this user");

            var tokenData = JsonSerializer.Deserialize<TokenDto>(storedToken);
            if (tokenData is null) return Unauthorized("Invalid refresh token data");

            if (tokenData.AccessToken != dto.RefreshToken) return Unauthorized("Invalid refresh token");
            if (tokenData.Expires >= TimeHelper.GetCurrentBangladeshTime())
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider: "Default", tokenName: "RefreshToken");
                return Unauthorized("Refresh token expired");
            }

            var newAccessToken = await _tokenService.GenerateJwtToken(user);
            return Ok(new AuthResponseDto
            {
                IsSuccess = true,
                Token = newAccessToken,
                RefreshToken = tokenData.RefreshToken,
                Expiration = DateTime.Now.AddMinutes(_jwtOptions.TokenValidityInMinutes),
                Message = "Token refreshed successfully"
            });
        }

        [Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");
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
    }

}