using Microsoft.AspNetCore.Mvc;
using WebScheduleApi.Models;
using WebScheduleApi.Repositories;

namespace WebScheduleApi.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController : ControllerBase
{
    private readonly ILessonsRepository _lessonsRepository;

    public LessonsController(ILessonsRepository lessonsRepository)
    {
        _lessonsRepository = lessonsRepository;
    }

    [HttpGet("{date}")]
    public async Task<IActionResult> GetLessons(string date)
    {
        try
        {
            var lessons = await _lessonsRepository.GetLessonsForDateAsync(date);
            return Ok(lessons);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving lessons: {ex.Message}");
        }
    }
}