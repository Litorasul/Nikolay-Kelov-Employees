namespace EmployeesOverlap.Server.Models
{
    public class EmployeePairOverlap
    {
        public int FirstEmployeeId { get; set; }
        public int SecondEmployeeId { get; set; }
        public int ProjectId { get; set; }
        public int DaysWorkedTogether { get; set; }
    }
}
