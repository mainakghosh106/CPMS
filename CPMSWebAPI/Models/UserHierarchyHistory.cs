namespace CPMSWebAPI.Models
{
    public class UserHierarchyHistory
    {
        public int Id { get; set; }
        public int SupervisorId { get; set; }
        public int SubordinateId { get; set; }
        public DateTime DeletedOn { get; set; }
        public string DeletedBy { get; set; }
    }
}
