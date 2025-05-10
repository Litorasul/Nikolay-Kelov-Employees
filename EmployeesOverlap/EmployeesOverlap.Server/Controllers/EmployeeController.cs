using EmployeesOverlap.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeesOverlap.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController: ControllerBase
    {
        private readonly IEmployeeOverlapService _overlapService;

        public EmployeeController(IEmployeeOverlapService overlapService)
        {
            _overlapService = overlapService;
        }

        [HttpPost("overlaps")]
        public async Task<IActionResult> CalculateOverlaps(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("CSV file is required.");

            var result = await _overlapService.ProcessFileAsync(file);
            return Ok(result);
        }
    }
}
