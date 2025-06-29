namespace CPMSWebAPI.Models
{
    public class TimeLog
    {
        public int Id { get; set; }

        public int TaskId { get; set; }
        public Tasks Task { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Date { get; set; }
        public double HoursSpent { get; set; }
    }
}
