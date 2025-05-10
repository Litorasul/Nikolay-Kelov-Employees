using EmployeesOverlap.Server.Services;

namespace EmployeesOverlap.Server.Tests
{
    public class CsvParserTests
    {
        private readonly ICsvParser _parser;
        public CsvParserTests()
        {
            _parser = new CsvParser();
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [Fact]
        public async Task ParseAsync_ValidCsv_ReturnsCorrectWorkRecords()
        {
            string csv = "EmpID,ProjectID,DateFrom,DateTo\n1,100,2025-01-01,2025-01-10\n2,100,2025-01-05,2025-01-12";
            var stream = GenerateStreamFromString(csv);

            var result = await _parser.ParseAsync(stream);

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].EmployeeId);
            Assert.Equal(new DateTime(2025, 1, 1), result[0].DateFrom);
        }

        [Fact]
        public async Task ParseAsync_MissingDateTo_ParsesAsNull()
        {
            string csv = "EmpID,ProjectID,DateFrom,DateTo\n1,200,2025-03-01,";
            var stream = GenerateStreamFromString(csv);

            var result = await _parser.ParseAsync(stream);

            Assert.Single(result);
            Assert.Equal(result[0].DateTo, DateTime.Today);
        }

        [Theory]
        [InlineData("01.03.2025", "02.03.2025")]
        [InlineData("3/1/2025", "3/2/2025")]
        [InlineData("2025-03-01", "2025-03-02")]
        public async Task ParseAsync_SupportsMultipleDateFormats(string dateFrom, string dateTo)
        {
            string csv = $"EmpID,ProjectID,DateFrom,DateTo\n1,200,{dateFrom},{dateTo}";
            var stream = GenerateStreamFromString(csv);

            var result = await _parser.ParseAsync(stream);

            Assert.Single(result);
            Assert.Equal(new DateTime(2025, 3, 1), result[0].DateFrom);
            Assert.Equal(new DateTime(2025, 3, 2), result[0].DateTo);
        }
        [Fact]
        public async Task ParseAsync_MissingRequiredColumn_Skips()
        {
            string csv = "EmpID,ProjectID, DateFrom,DateTo\n1,2025-01-01,2025-01-02";
            var stream = GenerateStreamFromString(csv);
            var result = await _parser.ParseAsync(stream);

            Assert.Empty(result);
        }
        [Fact]
        public async Task ParseAsync_InvalidDate_Skips()
        {
            string csv = "EmpID,ProjectID,DateFrom,DateTo\n1,200,notadate,2025-01-01";
            var stream = GenerateStreamFromString(csv);
            var result = await _parser.ParseAsync(stream);

            Assert.Empty(result);
        }
        [Fact]
        public async Task ParseAsync_EmptyFile_ReturnsEmptyList()
        {
            var stream = GenerateStreamFromString("");

            var result = await _parser.ParseAsync(stream);

            Assert.Empty(result);
        }
        [Fact]
        public async Task ParseAsync_DateFromBeforeDateTo_Skips()
        {
            string csv = "EmpID,ProjectID, DateFrom,DateTo\n1,2025-02-01,2025-01-02";
            var stream = GenerateStreamFromString(csv);
            var result = await _parser.ParseAsync(stream);

            Assert.Empty(result);
        }
    }
}
