using CPMSWebAPI.Models;

namespace CPMSWebAPI.DTO
{
    public class projectsDTO
    {
        public int Id { get; set; } = 0;
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }=DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string? Status { get; set; }
        public int CreatedByUserId { get; set; } = 0;
        public List<ProjectUserAssignmentDto>? AssignedUsers { get; set; } // New!

    }
}
