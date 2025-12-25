using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebScheduleApi.Models;

namespace WebScheduleApi.Controllers;

[ApiController]
[Route("api/homework")]
public class HomeworkController : ControllerBase
{
    private readonly DatabaseService _databaseService;

    public HomeworkController(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{date}/{order}")]
    public async Task<IActionResult> GetHomework(string date, int order)
    {
        try
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return BadRequest("Invalid date format. Use YYYY-MM-DD.");
            }

            var query = "SELECT homework_text FROM homework WHERE date = @date AND lesson_order = @order";
            var result = await _databaseService.ExecuteScalarAsync(query,
                new NpgsqlParameter("@date", parsedDate.Date),
                new NpgsqlParameter("@order", order));

            var homeworkText = result?.ToString() ?? string.Empty;

            var response = new HomeworkResponse
            {
                Date = date,
                Order = order,
                Homework = homeworkText,
                Success = true
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return Ok(new HomeworkResponse
            {
                Date = date,
                Order = order,
                Homework = string.Empty,
                Success = false,
                Message = $"Error retrieving homework: {ex.Message}"
            });
        }
    }

    [HttpPost("{date}/{order}")]
    public async Task<IActionResult> UpsertHomework(string date, int order, [FromBody] HomeworkRequest request)
    {
        try
        {
            if (!DateTime.TryParse(date, out var parsedDate))
            {
                return BadRequest("Invalid date format. Use YYYY-MM-DD.");
            }

            var query = @"
                INSERT INTO homework (date, lesson_order, homework_text)
                VALUES (@date, @order, @homework)
                ON CONFLICT (date, lesson_order)
                DO UPDATE SET homework_text = EXCLUDED.homework_text";

            await _databaseService.ExecuteNonQueryAsync(query,
                new NpgsqlParameter("@date", parsedDate.Date),
                new NpgsqlParameter("@order", order),
                new NpgsqlParameter("@homework", request.Homework ?? string.Empty));

            var response = new HomeworkResponse
            {
                Date = date,
                Order = order,
                Homework = request.Homework ?? string.Empty,
                Success = true
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return Ok(new HomeworkResponse
            {
                Date = date,
                Order = order,
                Homework = request.Homework ?? string.Empty,
                Success = false,
                Message = $"Error saving homework: {ex.Message}"
            });
        }
    }
}

public class HomeworkRequest
{
    public string? Homework { get; set; }
}