namespace CPMSWebAPI.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Projects Project { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public int AssignedToUserId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Status { get; set; }
        public double EstimatedHours { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
