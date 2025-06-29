namespace CPMSWebAPI.Models
{
    public class ProjectProgressLog
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Projects Project { get; set; }
        public string Comment { get; set; }
        public string? AttachmentPath { get; set; }
        public float? HoursSpent { get; set; }
        public int LogDoneBy { get; set; }
        public DateTime LogDate { get; set; } = DateTime.UtcNow;
    }
}
