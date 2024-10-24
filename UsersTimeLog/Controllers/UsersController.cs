using Microsoft.AspNetCore.Mvc;
using UsersTimeLog.Models;
using UsersTimeLog.Services;

namespace UsersTimeLog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private IUsersService _service;
        private ILogger<UsersController> _logger;

        public UsersController(IUsersService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<string>> GetUsersAsync(string start, string end, int pageNumber, int pageSize, string sortBy, string sortOrder, CancellationToken token)
        {
            try
            {
                // Calculate the number of rows to skip based on the page number and page size
                int skip = (pageNumber - 1) * pageSize;

                // Call the service method with the pagination and sorting parameters
                var timelogs = await _service.GetUsersAsync(start, end, skip, pageSize, sortBy, sortOrder, token);

                return Ok(timelogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users' timelogs");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("top")]
        public async Task<ActionResult<string>> GetTopUsersAsync(string start, string end, int topCount, CancellationToken token)
        {
            try
            {
                var timelogs = await _service.GetTopUsersAsync(start, end, topCount, token);
                return Ok(timelogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching top users' timelogs");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet("combined")]
        public async Task<IActionResult> GetCombinedUsersDataAsync([FromQuery] string? fromDate, [FromQuery] string? toDate, [FromQuery] int currentPage, [FromQuery] int? topCount, [FromQuery] string? sortColumn, [FromQuery] string? sortOrder, CancellationToken token)
        {
            try
            {
                int pageMaxSize = 10;
                currentPage = currentPage == 0 ? 1 : currentPage;

                // Calculate the number of rows to skip based on the page number and page size
                int skip = (currentPage - 1) * pageMaxSize;
                var usersTimelogs = await _service.GetUsersAsync(fromDate ?? "", toDate ?? "", skip == 0 ? 1 : skip, pageMaxSize, sortColumn, sortOrder, token);
                var totalUsers = usersTimelogs.TotalRecords;
                double pageCount = (double)((decimal)usersTimelogs.TotalRecords / Convert.ToDecimal(pageMaxSize));

                // Return the users and pagination details as a string
                var combinedResult = new CombinedModel
                {
                    UsersTimelogs = usersTimelogs.Timelogs,
                    TopUsersTimelogs = new List<Timelogs>(),
                    TotalPagescount = (int)Math.Ceiling(pageCount),
                    PageNumber = currentPage
                };

                return View("Index", combinedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching combined users' timelogs");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
