using CPMSWebAPI.DTO;
using CPMSWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CPMSWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Users> users { get; set; }
        public DbSet<Roles> Role { get; set; }
        public DbSet<UserHierarchy> UserHierarchy { get; set; }
        public DbSet<UserHierarchyHistory> userHierarchyHistory { get; set; }
        public DbSet<Projects> Project { get; set; }
        public DbSet<ProjectUser> ProjectUserMapping { get; set; }
        public DbSet<ProjectProgressLog> ProjectProgressLog { get; set; }
        public DbSet<DashboardSummaryDto> DashboardSummaryDto { get; set; }

        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<TimeLog> TimeLog { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserHierarchy>()
                .HasOne(uh => uh.Supervisor)
                .WithMany()
                .HasForeignKey(uh => uh.SupervisorId)
                .OnDelete(DeleteBehavior.NoAction); // Prevents cascade path

            modelBuilder.Entity<UserHierarchy>()
                .HasOne(uh => uh.Subordinate)
                .WithMany()
                .HasForeignKey(uh => uh.SubordinateId)
                .OnDelete(DeleteBehavior.NoAction); // Prevents cascade path

            modelBuilder.Entity<Roles>().HasData(
                new Roles() { Id = 1, RoleName = "Admin" },
                new Roles() { Id = 2, RoleName = "Manager" },
                new Roles() { Id = 3, RoleName = "TeamLead" }
            );

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.Project)
                .WithMany()
                .HasForeignKey(pu => pu.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);  // OK

            modelBuilder.Entity<ProjectUser>()
                .HasOne(pu => pu.User)
                .WithMany()
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.NoAction);  // Prevent conflict

           

        }
    }
}
