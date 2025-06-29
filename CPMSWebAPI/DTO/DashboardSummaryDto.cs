namespace CPMSWebAPI.DTO
{
    public class DashboardSummaryDto
    {
        public int TotalProjects { get; set; }
        public int TotalUsers { get; set; }
        public int TotalTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int MyTasks { get; set; }
        public int MyCompletedTasks { get; set; }
        public float MyHoursLogged { get; set; }
    }
}
