using EmployeesOverlap.Server.Models;

namespace EmployeesOverlap.Server.Services
{
    public interface ICsvParser
    {
        Task<List<WorkRecord>> ParseAsync(Stream stream);
    }
}