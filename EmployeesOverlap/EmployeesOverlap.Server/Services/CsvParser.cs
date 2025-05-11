using CsvHelper;
using System.Globalization;
using EmployeesOverlap.Server.Models;

namespace EmployeesOverlap.Server.Services
{
    public class CsvParser : ICsvParser
    {
        private static readonly string[] SupportedDateFormats = new[]
        {
            "yyyy-MM-dd",
            "dd-MM-yyyy",
            "MM/dd/yyyy",
            "dd.MM.yyyy",
            "yyyy.MM.dd",
            "d MMM yyyy",
            "dd MMM yyyy",
            "M/d/yyyy",
            "yyyy/MM/dd",
            "MMMM d yyyy",
            "d MMMM yyyy"
        };

        public async Task<List<WorkRecord>> ParseAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = new List<WorkRecord>();

            await foreach (var row in csv.GetRecordsAsync<dynamic>())
            {
                try
                {
                    var employeeId = int.Parse(row.EmpID);
                    var projectId = int.Parse(row.ProjectID);
                    var dateFrom = ParseFlexibleDate(row.DateFrom);
                    var dateToRaw = row.DateTo?.ToString()?.Trim();
                    var dateTo = string.IsNullOrEmpty(dateToRaw) || dateToRaw.ToLower() == "null"
                        ? DateTime.Today
                        : ParseFlexibleDate(dateToRaw);

                    if (dateFrom > dateTo)
                    {
                        Console.WriteLine($"Skipping row due to invalid date range: DateFrom ({dateFrom}) is after DateTo ({dateTo}).");
                        continue;
                    }

                    records.Add(new WorkRecord
                    {
                        EmployeeId = employeeId,
                        ProjectId = projectId,
                        DateFrom = dateFrom,
                        DateTo = dateTo
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Skipping row due to error: {ex.Message}");
                }
            }

            return records;
        }

        private DateTime ParseFlexibleDate(string input)
        {
            if (DateTime.TryParseExact(
                input.Trim(),
                SupportedDateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
            {
                return parsed;
            }

            throw new FormatException($"Unrecognized date format: {input}");
        }
    }
}
