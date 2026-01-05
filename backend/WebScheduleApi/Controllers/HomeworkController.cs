using Microsoft.AspNetCore.Mvc;
using WebScheduleApi.Models;
using WebScheduleApi.Repositories;

namespace WebScheduleApi.Controllers;

[ApiController]
[Route("api/homework")]
public class HomeworkController : ControllerBase
{
    private readonly IHomeworkRepository _homeworkRepository;

    public HomeworkController(IHomeworkRepository homeworkRepository)
    {
        _homeworkRepository = homeworkRepository;
    }

    [HttpGet("{date}/{order}")]
    public async Task<IActionResult> GetHomework(string date, int order)
    {
        if (string.IsNullOrEmpty(date))
        {
            return BadRequest("Date is required");
        }
        if (order <= 0)
        {
            return BadRequest("Order must be greater than 0");
        }

        try
        {
            var homeworkText = await _homeworkRepository.GetHomeworkAsync(date, order);

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
        if (string.IsNullOrEmpty(date))
        {
            return BadRequest("Date is required");
        }
        if (order <= 0)
        {
            return BadRequest("Order must be greater than 0");
        }
        if (request == null)
        {
            return BadRequest("Request body is required");
        }

        try
        {
            await _homeworkRepository.SaveHomeworkAsync(date, order, request.Homework ?? string.Empty);

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