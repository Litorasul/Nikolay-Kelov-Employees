using EmployeesOverlap.Server.Models;

namespace EmployeesOverlap.Server.Services
{
    public class EmployeeOverlapService : IEmployeeOverlapService
    {
        private readonly ICsvParser _csvParser;
        private readonly IOverlapCalculator _overlapCalculator;
        public EmployeeOverlapService(ICsvParser csvParser, IOverlapCalculator overlapCalculator)
        {
            _csvParser = csvParser;
            _overlapCalculator = overlapCalculator;
        }
        public async Task<List<EmployeePairOverlap>> ProcessFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid CSV file.");

            using var stream = file.OpenReadStream();
            var records = await _csvParser.ParseAsync(stream);

            var overlaps = _overlapCalculator.CalculateOverlaps(records);
            return overlaps;
        }
    }
}
