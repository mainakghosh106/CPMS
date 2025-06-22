using CPMSWebAPI.Data;
using CPMSWebAPI.DTO;
using CPMSWebAPI.Models;
using CPMSWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CPMSWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly IConfiguration _config;
        private readonly AppDbContext _appdbcontext;


        public LoginController(JwtService jwt, IConfiguration config, AppDbContext appDbContext)
        {
            _jwt = jwt;
            _config = config;
            _appdbcontext = appDbContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequests login)
        {
            if (login == null || string.IsNullOrEmpty(login.Password))
                return BadRequest("Invalid credentials.");

            var existingUser = await _appdbcontext.users.FirstOrDefaultAsync(u => u.UserName == login.Username);

            if (existingUser == null)
            {
                return Unauthorized("Invalid username");
            }

            var passwordHasher = new PasswordHasher<Users>();
            var result = passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, login.Password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid password.");


            var userWithRole = await _appdbcontext.users.Include(u => u.Role).Where(u => u.UserName == login.Username) 
            .Select(ur=>ur.Role.RoleName).FirstOrDefaultAsync();

           

            var accessToken = _jwt.GenerateAccessToken(login.Username, 1,userWithRole);
            var refreshToken = _jwt.GenerateRefreshToken(login.Username, 1, userWithRole);

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

            var newAccessToken = _jwt.GenerateAccessToken(usernameFromRefresh, 1,"");
            var newRefreshToken = _jwt.GenerateRefreshToken(usernameFromRefresh, 1,"");

            return Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
