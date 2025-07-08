using Application.Interfaces.Auth;
using Azure;
using Common.Utils.Helper;
using Core.Models.Auth;
using Core.ViewModels.Dto.Auth;
using Core.ViewModels.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Lead.Api.Controllers.Auth.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IOptions<JwtOptions> jwtOptions, IServiceTypeService service) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly JwtOptions _jwtOptions = jwtOptions.Value;
        private readonly IServiceTypeService _service = service;

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

                var token = await _tokenService.GenerateJwtToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

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
                    Expiration = TimeHelper.GetCurrentBangladeshTime().AddMinutes(_jwtOptions.TokenValidityInMinutes)
                };

                return Ok(ApiResponse<AuthResponseDto>.Success(response, "Login successful!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
            }
        }


        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto dto)
        {
            if (dto is null || string.IsNullOrEmpty(dto.RefreshToken) || string.IsNullOrEmpty(dto.AccessToken)) return BadRequest("Invalid client request!");

            // 1. Get claims principal from expired access token
            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
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

            var newAccessToken = await _tokenService.GenerateJwtToken(user);
            var response = new AuthResponseDto
            {
                Token = newAccessToken,
                RefreshToken = tokenData.RefreshToken,
                Expiration = TimeHelper.GetCurrentBangladeshTime().AddMinutes(_jwtOptions.TokenValidityInMinutes)
            };
            return Ok(ApiResponse<AuthResponseDto>.Success(response, "Token refreshed successful!"));
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

        [HttpGet]
        [Route("service-type")]
        public async Task<IActionResult> ServiceType()
        {
            try
            {
                var types = await _service.GetAspNetServiceTypesAsync();
                return Ok(ApiResponse<IList<AspNetServiceTypes>>.Success(types, "Token refreshed successful!"));
            }
            catch (Exception ex)
            {
                return Ok(ApiResponse<AuthResponseDto>.Fail("Something went wrong!"));
            }
        }
    }

}