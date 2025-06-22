using System.Text.Json.Serialization;

namespace CPMSWebAPI.Models
{
    public class UserHierarchy
    {
        public int Id { get; set; }

        public int SupervisorId { get; set; }
        public Users Supervisor { get; set; }

        public int SubordinateId { get; set; }
        public Users Subordinate { get; set; }
    }
}
