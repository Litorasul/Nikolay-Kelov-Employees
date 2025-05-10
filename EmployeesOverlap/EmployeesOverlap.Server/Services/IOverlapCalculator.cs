using EmployeesOverlap.Server.Models;

namespace EmployeesOverlap.Server.Services
{
    public interface IOverlapCalculator
    {
        List<EmployeePairOverlap> CalculateOverlaps(List<WorkRecord> workRecords);
    }
}