using CPMSWebAPI.Data;
using CPMSWebAPI.DTO;
using CPMSWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CPMSWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly AppDbContext _appdbcontext;
        public ProjectsController(AppDbContext appDbContext)
        {
            _appdbcontext = appDbContext;
        }

        [Authorize]
        [HttpPost("CreateProject")]
        public async Task<IActionResult> CreateProject([FromBody] projectsDTO projectsDTO)
        {
            if (projectsDTO == null)
            {
                return BadRequest("Details are not provided");
            }
            using var transaction = await _appdbcontext.Database.BeginTransactionAsync();
            try
            {

                var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

                //Insert into Projects table
                var lastProject = await _appdbcontext.Project.OrderByDescending(p => p.Id).FirstOrDefaultAsync();

                int newId = (lastProject?.Id ?? 0) + 1;
                string Code = "PRJ" + newId.ToString("D4");
                var project = new Projects
                {
                    Code = Code,
                    Name = projectsDTO.Name,
                    Description = projectsDTO.Description,
                    StartDate = projectsDTO.StartDate,
                    ActualStartDate = projectsDTO.StartDate,
                    ActualEndDate = projectsDTO.EndDate,
                    EndDate = projectsDTO.EndDate,
                    Status = "Pending",
                    CreatedByUserId = userId
                };
                _appdbcontext.Project.Add(project);
                await _appdbcontext.SaveChangesAsync();

                // 🔁 Assign users to the project
                foreach (var assignment in projectsDTO.AssignedUsers)
                {
                    var projectUser = new ProjectUser
                    {
                        ProjectId = project.Id,
                        UserId = assignment.UserId,
                        AssignedOn = DateTime.Now
                    };
                    _appdbcontext.ProjectUserMapping.Add(projectUser);
                }

                await _appdbcontext.SaveChangesAsync();


                await transaction.CommitAsync();

                return Ok("Project created successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("GetProjectListUserwise")]
        public async Task<IActionResult> GetProjectListUserwise()
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            var Projectlist = await _appdbcontext.ProjectUserMapping
                .Include(u => u.Project)
                .Include(u => u.User)
                .ThenInclude(p => p.Role)
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.ProjectId,
                    u.Project.Code,
                    u.Project.Name,
                    u.Project.Status,
                    u.Project.StartDate,
                    u.Project.EndDate,
                    userRole = u.User.Role.RoleName
                }).
                ToListAsync();
            return Ok(Projectlist);

        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("UpdateProjectStatus")]
        public async Task<IActionResult> UpdateProjectStatus(projectsDTO dto)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            if (dto == null || dto.Id <= 0 || string.IsNullOrWhiteSpace(dto.Status))
            {
                return BadRequest("Invalid project details.");
            }

            var project = await _appdbcontext.Project.FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (project == null)
            {
                return NotFound("Project not found.");
            }

            if (project.Status == "Completed")
            {
                if (dto.ActualEndDate == null)
                {
                    return BadRequest("Please enter actual end date as status is completed.");
                }
                else
                {
                    project.ActualEndDate = dto.ActualEndDate;
                }

            }
            using var transaction = await _appdbcontext.Database.BeginTransactionAsync();
            try
            {

                project.Status = dto.Status;
                project.ModifiedByUserId = userId;
                _appdbcontext.Project.Update(project);
                await _appdbcontext.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok("Project status updated successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error updating status: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("InsertProjectLog")]
        public async Task<IActionResult> InsertProjectLog([FromBody] Projectprogresslogdto projectprogressDTO)
        {
            if (projectprogressDTO == null)
            {
                return BadRequest("Details are not provided");
            }
            using var transaction = await _appdbcontext.Database.BeginTransactionAsync();
            try
            {

                var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

                //Insert into Project log table
                var Project = await _appdbcontext.Project.FirstOrDefaultAsync(p => p.Id==projectprogressDTO.ProjectId);

                if(Project == null)
                {
                    return BadRequest("Project does not exists");
                }
                bool isUserMapped = await _appdbcontext.ProjectUserMapping.AnyAsync(x => x.ProjectId == projectprogressDTO.ProjectId && x.UserId == userId);

                if (!isUserMapped)
                {
                    return Forbid("You are not assigned to this project.");
                }
                if (projectprogressDTO.HoursSpent <= 0)
                {
                    return BadRequest("Hours spent must be greater than zero.");
                }
                if (string.IsNullOrWhiteSpace(projectprogressDTO.Comment))
                {
                    return BadRequest("Comment is required.");
                }
                if (Project.Status == "Completed")
                {
                    return BadRequest("Cannot add log. Project is already completed.");
                }
                string? savedFilePath = null;
                if (projectprogressDTO.AttachmentPath != null)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "ProjectLog");

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(projectprogressDTO.AttachmentPath.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await projectprogressDTO.AttachmentPath.CopyToAsync(stream);
                    }

                    savedFilePath = Path.Combine("Uploads", "ProjectLog", uniqueFileName).Replace("\\", "/");
                }

                var projectlog = new ProjectProgressLog
                {
                    ProjectId = projectprogressDTO.ProjectId,
                    Comment=    projectprogressDTO.Comment,
                    AttachmentPath= savedFilePath,
                    HoursSpent= projectprogressDTO.HoursSpent,
                    LogDoneBy= userId

                };
                _appdbcontext.ProjectProgressLog.Add(projectlog);
                await _appdbcontext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok("Project log submitted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("GetAllProjectAssignments")]
        public async Task<IActionResult> GetAllProjectAssignments()
        {
            var result = await _appdbcontext.Project
                .Select(project => new
                {
                    ProjectName = project.Name,
                    ProjectCode = project.Code,
                    Status = project.Status,
                    AssignedUsers = _appdbcontext.ProjectUserMapping
                        .Where(pum => pum.ProjectId == project.Id)
                        .Select(pum => new
                        {
                            UserName = _appdbcontext.users
                                .Where(u => u.Id == pum.UserId)
                                .Select(u => u.UserName)
                                .FirstOrDefault(),
                            Role = _appdbcontext.Role
                                .Where(r => r.Id == pum.Id)
                                .Select(r => r.RoleName)
                                .FirstOrDefault(),
                            Logs = _appdbcontext.ProjectProgressLog
                                .Where(log => log.ProjectId == project.Id && log.LogDoneBy == pum.UserId)
                                .Select(log => new
                                {
                                    log.Comment,
                                    log.HoursSpent,
                                    log.AttachmentPath,
                                    log.LogDate
                                }).ToList()
                        }).ToList()
                })
                .ToListAsync();

            return Ok(result);
        }



    }
}
