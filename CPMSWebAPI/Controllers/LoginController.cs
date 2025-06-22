using CPMSWebAPI.Models;
using CPMSWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace CPMSWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly IConfiguration _config;


        public LoginController(JwtService jwt, IConfiguration config)
        {
            _jwt = jwt;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequests login)
        {
            // Hardcoded demo user for now
            if (login.Username != "admin" || login.Password != "password")
                return Unauthorized();

            var accessToken = _jwt.GenerateAccessToken(login.Username, 1);
            var refreshToken = _jwt.GenerateRefreshToken(login.Username, 1);

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] TokenRefreshRequest request)
        {
            var principal = _jwt.GetPrincipalFromToken(request.AccessToken, isRefreshToken: false);
            var refreshPrincipal = _jwt.GetPrincipalFromToken(request.RefreshToken, isRefreshToken: true);

            if (principal == null || refreshPrincipal == null)
                return Unauthorized("Invalid tokens");

            var usernameFromAccess = principal.Identity.Name;
            var usernameFromRefresh = refreshPrincipal.Identity.Name;

            if (usernameFromAccess != usernameFromRefresh)
                return Unauthorized("Mismatched user");

            var newAccessToken = _jwt.GenerateAccessToken(usernameFromRefresh, 1);
            var newRefreshToken = _jwt.GenerateRefreshToken(usernameFromRefresh, 1);

            return Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
