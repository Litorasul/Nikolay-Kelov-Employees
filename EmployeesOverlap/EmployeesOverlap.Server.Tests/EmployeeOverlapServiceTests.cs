using Moq;
using System.Text;
using Microsoft.AspNetCore.Http;
using EmployeesOverlap.Server.Models;
using EmployeesOverlap.Server.Services;

namespace EmployeesOverlap.Server.Tests
{
    public class EmployeeOverlapServiceTests
    {
        private readonly Mock<ICsvParser> _mockCsvParser;
        private readonly Mock<IOverlapCalculator> _mockOverlapCalculator;
        private readonly EmployeeOverlapService _service;
        public EmployeeOverlapServiceTests()
        {
            _mockCsvParser = new Mock<ICsvParser>();
            _mockOverlapCalculator = new Mock<IOverlapCalculator>();
            _service = new EmployeeOverlapService(_mockCsvParser.Object, _mockOverlapCalculator.Object);
        }
        private IFormFile CreateFakeFormFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", "test.csv");
        }
        [Fact]
        public async Task ProcessFileAsync_ValidFile_ReturnsExpectedOverlaps()
        {
            var file = CreateFakeFormFile("valid csv content");

            var parsedRecords = new List<WorkRecord>
            {
                new WorkRecord { EmployeeId = 1, ProjectId = 101, DateFrom = DateTime.Today, DateTo = DateTime.Today.AddDays(5) },
                new WorkRecord { EmployeeId = 2, ProjectId = 101, DateFrom = DateTime.Today, DateTo = DateTime.Today.AddDays(3) }
            };

            var expectedOverlaps = new List<EmployeePairOverlap>
            {
                new EmployeePairOverlap { FirstEmployeeId = 1, SecondEmployeeId = 2, ProjectId = 101, DaysWorkedTogether = 4 }
            };

            _mockCsvParser.Setup(p => p.ParseAsync(It.IsAny<Stream>()))
                          .ReturnsAsync(parsedRecords);

            _mockOverlapCalculator.Setup(c => c.CalculateOverlaps(parsedRecords))
                                  .Returns(expectedOverlaps);

            var result = await _service.ProcessFileAsync(file);

            Assert.Single(result);
            Assert.Equal(1, result[0].FirstEmployeeId);
            Assert.Equal(2, result[0].SecondEmployeeId);
            Assert.Equal(101, result[0].ProjectId);
            Assert.Equal(4, result[0].DaysWorkedTogether);
        }
        [Fact]
        public async Task ProcessFileAsync_NullFile_ThrowsArgumentException()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.ProcessFileAsync(null));
            Assert.Equal("Invalid CSV file.", ex.Message);
        }
        [Fact]
        public async Task ProcessFileAsync_EmptyFile_ThrowsArgumentException()
        {
            var emptyStream = new MemoryStream();
            var file = new FormFile(emptyStream, 0, 0, "file", "empty.csv");

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.ProcessFileAsync(file));
            Assert.Equal("Invalid CSV file.", ex.Message);
        }
        [Fact]
        public async Task ProcessFileAsync_ParserThrows_Throws()
        {
            var file = CreateFakeFormFile("invalid csv");

            _mockCsvParser.Setup(p => p.ParseAsync(It.IsAny<Stream>()))
                          .ThrowsAsync(new FormatException("CSV format error"));

            var ex = await Assert.ThrowsAsync<FormatException>(() => _service.ProcessFileAsync(file));
            Assert.Equal("CSV format error", ex.Message);
        }
        [Fact]
        public async Task ProcessFileAsync_NoRecords_ReturnsEmptyList()
        {
            var file = CreateFakeFormFile("header only");

            _mockCsvParser.Setup(p => p.ParseAsync(It.IsAny<Stream>()))
                          .ReturnsAsync(new List<WorkRecord>());

            _mockOverlapCalculator.Setup(c => c.CalculateOverlaps(It.IsAny<List<WorkRecord>>()))
                                  .Returns(new List<EmployeePairOverlap>());

            var result = await _service.ProcessFileAsync(file);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
