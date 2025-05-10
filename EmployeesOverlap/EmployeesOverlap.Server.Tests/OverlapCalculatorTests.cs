using EmployeesOverlap.Server.Models;
using EmployeesOverlap.Server.Services;

namespace EmployeesOverlap.Server.Tests
{
    public class OverlapCalculatorTests
    {
        private readonly IOverlapCalculator _overlapCalculator;

        public OverlapCalculatorTests()
        {
            _overlapCalculator = new OverlapCalculator();
        }
        
        [Fact]
        public void CalculateOverlaps_SingleEmployee_NoOverlap()
        {
            var workRecords = new List<WorkRecord> 
            { 
                new WorkRecord { EmployeeId = 1, ProjectId = 100, DateFrom = new DateTime(2025, 01, 01), DateTo = new DateTime(2025, 01, 10) } 
            };


            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            Assert.Empty(result); // No overlaps since there's only one employee
        }

        [Fact]
        public void CalculateOverlaps_TwoEmployees_Overlap()
        {
            var workRecords = new List<WorkRecord>
            {
                new WorkRecord { EmployeeId = 1, ProjectId = 100, DateFrom = new DateTime(2025, 01, 01), DateTo = new DateTime(2025, 01, 10) },
                new WorkRecord { EmployeeId = 2, ProjectId = 100, DateFrom = new DateTime(2025, 01, 05), DateTo = new DateTime(2025, 01, 12) }
            };

            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            Assert.Single(result);
            Assert.Equal(1, result[0].FirstEmployeeId);  // Order by EmployeeId
            Assert.Equal(2, result[0].SecondEmployeeId);
            Assert.Equal(100, result[0].ProjectId);
            Assert.Equal(6, result[0].DaysWorkedTogether); // Overlap from Jan 5 to Jan 10
        }
        [Fact]
        public void CalculateOverlaps_MultipleEmployees_Overlapping()
        {
            var workRecords = new List<WorkRecord>
            {
                new WorkRecord { EmployeeId = 1, ProjectId = 100, DateFrom = new DateTime(2025, 01, 01), DateTo = new DateTime(2025, 01, 10) },
                new WorkRecord { EmployeeId = 2, ProjectId = 100, DateFrom = new DateTime(2025, 01, 05), DateTo = new DateTime(2025, 01, 12) },
                new WorkRecord { EmployeeId = 3, ProjectId = 100, DateFrom = new DateTime(2025, 01, 06), DateTo = new DateTime(2025, 01, 15) }
            };

            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            // Check overlap for employee 1 and 2
            var overlap12 = result.FirstOrDefault(o => o.FirstEmployeeId == 1 && o.SecondEmployeeId == 2);
            Assert.NotNull(overlap12);
            Assert.Equal(6, overlap12.DaysWorkedTogether); 

            // Check overlap for employee 1 and 3
            var overlap13 = result.FirstOrDefault(o => o.FirstEmployeeId == 1 && o.SecondEmployeeId == 3);
            Assert.NotNull(overlap13);
            Assert.Equal(5, overlap13.DaysWorkedTogether); 

            // Check overlap for employee 2 and 3
            var overlap23 = result.FirstOrDefault(o => o.FirstEmployeeId == 2 && o.SecondEmployeeId == 3);
            Assert.NotNull(overlap23);
            Assert.Equal(7, overlap23.DaysWorkedTogether); 
        }
        [Fact]
        public void CalculateOverlaps_NoOverlap()
        {
            var workRecords = new List<WorkRecord>
            {
                new WorkRecord { EmployeeId = 1, ProjectId = 100, DateFrom = new DateTime(2025, 01, 01), DateTo = new DateTime(2025, 01, 05) },
                new WorkRecord { EmployeeId = 2, ProjectId = 100, DateFrom = new DateTime(2025, 01, 06), DateTo = new DateTime(2025, 01, 10) }
            };

            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            Assert.Empty(result); 
        }
        [Fact]
        public void CalculateOverlaps_FullOverlap()
        {
            var workRecords = new List<WorkRecord>
            {
                new WorkRecord { EmployeeId = 1, ProjectId = 100, DateFrom = new DateTime(2025, 01, 01), DateTo = new DateTime(2025, 01, 10) },
                new WorkRecord { EmployeeId = 2, ProjectId = 100, DateFrom = new DateTime(2025, 01, 01), DateTo = new DateTime(2025, 01, 10) }
            };

            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            Assert.Single(result);
            Assert.Equal(10, result[0].DaysWorkedTogether); 
        }
        [Fact]
        public void CalculateOverlaps_EmptyList_ReturnsEmpty()
        {
            var workRecords = new List<WorkRecord>();

            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            Assert.Empty(result);
        }
        [Fact]
        public void CalculateOverlaps_LargeDataset_PerformanceTest()
        {
            var workRecords = new List<WorkRecord>();

            for (int i = 1; i <= 1000; i++)
            {
                workRecords.Add(new WorkRecord
                {
                    EmployeeId = i,
                    ProjectId = 100,
                    DateFrom = new DateTime(2025, 01, 01),
                    DateTo = new DateTime(2025, 01, 10)
                });
            }

            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            Assert.True(result.Count > 0); 
        }
        [Fact]
        public void CalculateOverlaps_NewYearBoundary_Overlap()
        {
            var workRecords = new List<WorkRecord>
            {
                new WorkRecord { EmployeeId = 1, ProjectId = 100, DateFrom = new DateTime(2024, 12, 30), DateTo = new DateTime(2025, 01, 10) },
                new WorkRecord { EmployeeId = 2, ProjectId = 100, DateFrom = new DateTime(2025, 01, 05), DateTo = new DateTime(2025, 01, 15) }
            };

            var result = _overlapCalculator.CalculateOverlaps(workRecords);

            Assert.Single(result);
            Assert.Equal(6, result[0].DaysWorkedTogether);
        }
    }
}
