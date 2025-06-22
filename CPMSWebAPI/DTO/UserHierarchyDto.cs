namespace CPMSWebAPI.DTO
{
    public class UserHierarchyDto
    {
        public int Id { get; set; }
        public int SupervisorId { get; set; }
        public int SubordinateId { get; set; }
    }
}
