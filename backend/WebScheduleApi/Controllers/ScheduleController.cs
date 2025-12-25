using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace WebScheduleApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly DatabaseService _databaseService;

    public ScheduleController(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello from WebScheduleApi");
    }

    [HttpGet("test-query")]
    public async Task<IActionResult> TestQuery()
    {
        try
        {
            // Example query - replace with actual query
            var result = await _databaseService.ExecuteScalarAsync("SELECT 1");
            return Ok(new { result });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database error: {ex.Message}");
        }
    }
}