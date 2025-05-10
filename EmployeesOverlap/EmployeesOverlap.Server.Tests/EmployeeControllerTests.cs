using Moq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EmployeesOverlap.Server.Controllers;
using EmployeesOverlap.Server.Models;
using EmployeesOverlap.Server.Services;

namespace EmployeesOverlap.Server.Tests
{
    public class EmployeeControllerTests
    {
        private readonly Mock<IEmployeeOverlapService> _mockService;
        private readonly EmployeeController _controller;

        public EmployeeControllerTests()
        {
            _mockService = new Mock<IEmployeeOverlapService>();
            _controller = new EmployeeController(_mockService.Object);
        }
        private IFormFile CreateFakeFormFile(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, bytes.Length, "file", "test.csv");
        }
        [Fact]
        public async Task CalculateOverlaps_ValidFile_ReturnsOkWithResult()
        {
            var file = CreateFakeFormFile("valid content");
            var expected = new List<EmployeePairOverlap>
            {
                new EmployeePairOverlap
                {
                    FirstEmployeeId = 1,
                    SecondEmployeeId = 2,
                    ProjectId = 101,
                    DaysWorkedTogether = 5
                }
            };

            _mockService.Setup(s => s.ProcessFileAsync(file)).ReturnsAsync(expected);

            var result = await _controller.CalculateOverlaps(file) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var returnedList = Assert.IsType<List<EmployeePairOverlap>>(result.Value);
            Assert.Single(returnedList);
            Assert.Equal(1, returnedList[0].FirstEmployeeId);
        }
        [Fact]
        public async Task CalculateOverlaps_NullFile_ReturnsBadRequest()
        {
            var result = await _controller.CalculateOverlaps(null) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("CSV file is required.", result.Value);
        }
        [Fact]
        public async Task CalculateOverlaps_EmptyFile_ReturnsBadRequest()
        {
            var file = new FormFile(new MemoryStream(), 0, 0, "file", "empty.csv");

            var result = await _controller.CalculateOverlaps(file) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("CSV file is required.", result.Value);
        }
        [Fact]
        public async Task CalculateOverlaps_ServiceThrows_ExceptionBubblesUp()
        {
            var file = CreateFakeFormFile("some content");
            _mockService.Setup(s => s.ProcessFileAsync(file)).ThrowsAsync(new InvalidOperationException("Parsing failed"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.CalculateOverlaps(file));
        }
        [Fact]
        public async Task CalculateOverlaps_CallsServiceOnce()
        {
            var file = CreateFakeFormFile("csv data");
            _mockService.Setup(s => s.ProcessFileAsync(file)).ReturnsAsync(new List<EmployeePairOverlap>());

            await _controller.CalculateOverlaps(file);

            _mockService.Verify(s => s.ProcessFileAsync(file), Times.Once);
        }
    }
}
