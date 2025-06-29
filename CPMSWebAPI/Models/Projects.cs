namespace CPMSWebAPI.Models
{
    public class Projects
    {
        public int Id { get; set; }
        public string Code { get; set; } // Auto-generated project code
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public string Status { get; set; }
        public int CreatedByUserId { get; set; }
        public Users CreatedByUser { get; set; }
        public int ModifiedByUserId { get; set; }
        public Users ModifiedByUser { get; set; }
    }
}
