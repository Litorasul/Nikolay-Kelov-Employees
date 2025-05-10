using EmployeesOverlap.Server.Models;

namespace EmployeesOverlap.Server.Services
{
    public class OverlapCalculator : IOverlapCalculator
    {
        private DateTime Max(DateTime a, DateTime b) => a > b ? a : b;
        private DateTime Min(DateTime a, DateTime b) => a < b ? a : b;
        public List<EmployeePairOverlap> CalculateOverlaps(List<WorkRecord> workRecords)
        {
            var result = new List<EmployeePairOverlap>();

            // Group by project
            var projects = workRecords
                .GroupBy(wr => wr.ProjectId);

            foreach (var projectGroup in projects)
            {
                var records = projectGroup.ToList();

                for (int i = 0; i < records.Count - 1; i++)
                {
                    for (int j = i + 1; j < records.Count; j++)
                    {
                        var a = records[i];
                        var b = records[j];

                        // Calculate overlapping period
                        var overlapStart = Max(a.DateFrom, b.DateFrom);
                        var overlapEnd = Min(a.DateTo ?? DateTime.Today, b.DateTo ?? DateTime.Today);

                        if (overlapEnd >= overlapStart)
                        {
                            var days = (overlapEnd - overlapStart).Days + 1;

                            result.Add(new EmployeePairOverlap
                            {
                                FirstEmployeeId = Math.Min(a.EmployeeId, b.EmployeeId),
                                SecondEmployeeId = Math.Max(a.EmployeeId, b.EmployeeId),
                                ProjectId = projectGroup.Key,
                                DaysWorkedTogether = days
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
