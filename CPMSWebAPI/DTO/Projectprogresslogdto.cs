using CPMSWebAPI.Models;

namespace CPMSWebAPI.DTO
{
    public class Projectprogresslogdto
    {
        public int Id { get; set; } = 0;
        public int ProjectId { get; set; } = 0;
        public Projects? Project { get; set; }
        public string? Comment { get; set; }
        public IFormFile? AttachmentPath { get; set; }
        public float? HoursSpent { get; set; }
        public string? LogDoneBy { get; set; }
        public DateTime? LogDate { get; set; }
    }
}
