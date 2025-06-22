namespace CPMSWebAPI.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string? CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public Roles? Role { get; set; }
    }
}
