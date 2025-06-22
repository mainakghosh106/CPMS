using CPMSWebAPI.Data;
using CPMSWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CPMSWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appdbcontext;

        public UserController(AppDbContext appDbContext)
        {
            _appdbcontext = appDbContext;
        }


        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUserList()
        {
            var data = await _appdbcontext.users.ToListAsync();
            return Ok(data);
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser([FromBody] Users users)
        {
            if (users == null)
                return BadRequest("User data is required.");

            var existingUser = await _appdbcontext.users
                .FirstOrDefaultAsync(u => u.UserName == users.UserName || u.Email == users.Email);

            if (existingUser != null)
                return Conflict("A user with the same UserId or Email already exists.");

            try
            {
                PasswordHasher<Users> passwordHasher = new PasswordHasher<Users>();
                users.PasswordHash = passwordHasher.HashPassword(users, users.PasswordHash);
                users.CreatedOn = DateTime.Now.ToString();

                await _appdbcontext.users.AddAsync(users);
                await _appdbcontext.SaveChangesAsync();

                return CreatedAtAction(nameof(RegisterUser), new { id = users.Id }, users); // HTTP 201
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }

        [HttpPost("RegisterUserBulk")]
        public async Task<IActionResult> BulkRegisterUser(List<Users> users)
        {
            if (users == null)
                return BadRequest("User data is required.");
            try
            {
                foreach (var d in users)
                {

                    var existingUser = await _appdbcontext.users
                    .FirstOrDefaultAsync(u => u.UserName == d.UserName || u.Email == d.Email);

                    if (existingUser != null)
                        return Conflict("A user with the same UserId or Email already exists.");


                    PasswordHasher<Users> passwordHasher = new PasswordHasher<Users>();
                    d.PasswordHash = passwordHasher.HashPassword(d, d.PasswordHash);
                    d.CreatedOn = DateTime.Now.ToString();

                    await _appdbcontext.users.AddAsync(d);
                    await _appdbcontext.SaveChangesAsync();


                }
                return Ok(users);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        public async Task<IActionResult> UpdateUserDetails([FromBody] Users users)
        {
            if(users.Id==0)
            {
                return NotFound();
            }
            var existingUser = await _appdbcontext.users
                    .FirstOrDefaultAsync(u => u.Id == users.Id);

            if (existingUser == null)
            {
                return Conflict("This user does not exists.");
            }
            PasswordHasher<Users> passwordHasher = new PasswordHasher<Users>();
            users.PasswordHash = passwordHasher.HashPassword(users, users.PasswordHash);
            await _appdbcontext.users
                .Where(u => u.Id == users.Id)
                .ExecuteUpdateAsync(setters => setters
                .SetProperty(u => u.Email, users.Email)
                .SetProperty(u => u.PasswordHash, users.PasswordHash)
                .SetProperty(u => u.Role, users.Role)
            );
            await _appdbcontext.SaveChangesAsync();
            return Ok(users);
        }
    }
}
