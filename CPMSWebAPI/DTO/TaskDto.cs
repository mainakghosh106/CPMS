namespace CPMSWebAPI.DTO
{
    public class TaskDto
    {
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double EstimatedHours { get; set; }
        public int AssignedToUser { get; set; }
    }
}
