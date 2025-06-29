using CPMSWebAPI.Data;
using CPMSWebAPI.DTO;
using CPMSWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CPMSWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _appdbcontext;

        public TaskController(AppDbContext appDbContext)
        {
            _appdbcontext = appDbContext; 
        }

        [Authorize(Roles ="Manager,TeamLead")]
        [HttpPost("CreateAndAssignTask")]

        public async Task<IActionResult> CreateTaskandAssignEmployees(TaskDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Details are not provided");
            }
            using var transaction = await _appdbcontext.Database.BeginTransactionAsync();
            try
            {

                var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

                //Insert into Task table
                var Project = await _appdbcontext.Project.FirstOrDefaultAsync(p => p.Id == dto.ProjectId);

                if (Project == null)
                {
                    return BadRequest("Project does not exists");
                }
                bool isUserMapped = await _appdbcontext.ProjectUserMapping.AnyAsync(x => x.ProjectId == dto.ProjectId && x.UserId == userId);

                if (!isUserMapped)
                {
                    return Forbid("this employee is not assigned to this project.");
                }
                if (dto.EstimatedHours <= 0)
                {
                    return BadRequest("Estmated hours must be greater than zero.");
                }
                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    return BadRequest("Title is required.");
                }
                if (string.IsNullOrWhiteSpace(dto.Description))
                {
                    return BadRequest("Description is required.");
                }
                if (Project.Status == "Completed")
                {
                    return BadRequest("Cannot add log. Project is already completed.");
                }
                
                var task = new Tasks
                {
                    ProjectId = dto.ProjectId,
                    Title = dto.Title,
                    Description = dto.Description,
                    AssignedToUserId=dto.AssignedToUser,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    EstimatedHours= dto.EstimatedHours,
                    CreatedBy=userId,
                    CreatedOn=DateTime.UtcNow

                };
                _appdbcontext.Tasks.Add(task);
                await _appdbcontext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok("Task added successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "TeamLead,Employee")]
        [HttpPost("UpdateTaskLog")]

        public async Task<IActionResult> TaskUpdate(TimeLogDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Details are not provided");
            }
            using var transaction = await _appdbcontext.Database.BeginTransactionAsync();
            try
            {

                var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);

                //Insert into timelog table
                var Task = await _appdbcontext.Tasks.FirstOrDefaultAsync(p => p.Id == dto.TaskId);
                var taskWithProject = await _appdbcontext.Tasks
                .Include(t => t.Project)
                .Where(t => t.Id == dto.TaskId)
                .Select(e => e.Project.Status)
                .FirstOrDefaultAsync();
                if (Task == null)
                {
                    return BadRequest("Task does not exists");
                }
                bool isUserMapped = await _appdbcontext.Tasks.AnyAsync(x => x.Id == dto.TaskId && x.AssignedToUserId == userId);

                if (!isUserMapped)
                {
                    return Forbid("You are not assigned to this task.");
                }
                if (dto.HoursSpent <= 0)
                {
                    return BadRequest("Spent hours must be greater than zero.");
                }
               
                if (taskWithProject == "Completed")
                {
                    return BadRequest("Cannot add log. Project is already completed.");
                }

                var timelog = new TimeLog
                {
                    TaskId = dto.TaskId,
                    EmployeeId = userId,
                    Date = dto.Date,
                    HoursSpent = dto.HoursSpent
                };
                _appdbcontext.TimeLog.Add(timelog);
                await _appdbcontext.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok("Time log added successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
