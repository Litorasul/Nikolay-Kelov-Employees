using EmployeesOverlap.Server.Models;

namespace EmployeesOverlap.Server.Services
{
    public interface IEmployeeOverlapService
    {
        Task<List<EmployeePairOverlap>> ProcessFileAsync(IFormFile file);
    }
}