using CPMSWebAPI.Data;
using CPMSWebAPI.DTO;
using CPMSWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUserList()
        {
            var usersWithRoles = await _appdbcontext.users.Include(u => u.Role)
            .Select(u => new
            {
                Name = u.UserName,
                Email = u.Email,
                RoleName = u.Role.RoleName,
                Status = u.IsActive ? "Active" : "Not Active"
            }).ToListAsync();
            return Ok(usersWithRoles);
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
            if (users.Id == 0)
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

        [Authorize(Roles ="Admin")]
        [HttpPost("AssignHierarchy")]
        public async Task<IActionResult> AssignHierarchy([FromBody] UserHierarchyDto userHierarchyDTO)
        {
            if (userHierarchyDTO == null)
            {
                return BadRequest("User data is required.");
            }

            if (userHierarchyDTO.SupervisorId == userHierarchyDTO.SubordinateId)
            {
                return BadRequest("Supervisor and Subordinate cannot be the same user.");
            }

            var supervisor = await _appdbcontext.users.FindAsync(userHierarchyDTO.SupervisorId);
            var subordinate = await _appdbcontext.users.FindAsync(userHierarchyDTO.SubordinateId);

            if (supervisor == null || subordinate == null)
            {
                return NotFound("User details not found");
            }

            bool exists = await _appdbcontext.UserHierarchy.AnyAsync(x => x.SupervisorId == userHierarchyDTO.SupervisorId && x.SubordinateId == userHierarchyDTO.SubordinateId);
            if (exists)
            {
                return Conflict("This supervisor-subordinate relationship already exists.");
            }

            var relation = new UserHierarchy
            {
                SupervisorId = userHierarchyDTO.SupervisorId,
                SubordinateId = userHierarchyDTO.SubordinateId
            };

            _appdbcontext.UserHierarchy.Add(relation);
            await _appdbcontext.SaveChangesAsync();

            return Ok("User hierarchy assigned successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("DeleteHierarchy")]
        public async Task<IActionResult> DeleteHierarchy([FromBody] UserHierarchyDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Details are not provided");
            }
            using var transaction = await _appdbcontext.Database.BeginTransactionAsync();
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                //Insert into history
                var relation = new UserHierarchyHistory
                {
                    SupervisorId = dto.SupervisorId,
                    SubordinateId = dto.SubordinateId,
                    DeletedOn = DateTime.Now,
                    DeletedBy= userId
                };

                _appdbcontext.userHierarchyHistory.Add(relation);
                await _appdbcontext.SaveChangesAsync();

                //delete the hierarchy

                var rowsAffected = await _appdbcontext.UserHierarchy.Where(x => x.Id == dto.Id).ExecuteDeleteAsync();

                if (rowsAffected == 0)
                    return NotFound("No matching hierarchy found.");

                await transaction.CommitAsync();

                return Ok("Hierarchy deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
