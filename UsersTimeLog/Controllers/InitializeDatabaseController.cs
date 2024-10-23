using Microsoft.AspNetCore.Mvc;
using UsersTimeLog.Services;

namespace UsersTimeLog.Controllers;

[ApiController]
[Route("api/system")]
public class InitializeDatabaseController : ControllerBase
{
    private ISystemService _service;
    private ILogger<InitializeDatabaseController> _logger;

    public InitializeDatabaseController(ISystemService service, ILogger<InitializeDatabaseController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet("db-init")]
    public async Task<ActionResult<string>> InitializeDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _service.InitializeDatabaseAsync(cancellationToken);

            return Ok("result");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialize database");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}

