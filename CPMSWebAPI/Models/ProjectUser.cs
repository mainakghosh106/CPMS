namespace CPMSWebAPI.Models
{
    public class ProjectUser
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Projects Project { get; set; }

        public int UserId { get; set; }
        public Users User { get; set; }
        public DateTime AssignedOn { get; set; }
    }
}
