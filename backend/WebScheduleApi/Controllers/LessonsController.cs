using Microsoft.AspNetCore.Mvc;
using WebScheduleApi.Models;
using WebScheduleApi.Repositories;

namespace WebScheduleApi.Controllers;

[ApiController]
[Route("api/lessons")]
public class LessonsController : ControllerBase
{
    private readonly ILessonsRepository _lessonsRepository;
    private readonly ILessonDetailRepository _lessonDetailRepository;

    public LessonsController(ILessonsRepository lessonsRepository, ILessonDetailRepository lessonDetailRepository)
    {
        _lessonsRepository = lessonsRepository;
        _lessonDetailRepository = lessonDetailRepository;
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

    [HttpGet("{date}/{order}")]
    public async Task<IActionResult> GetLessonDetail(string date, int order)
    {
        try
        {
            var lessonDetail = await _lessonDetailRepository.GetLessonDetailAsync(date, order);
            if (lessonDetail == null)
            {
                return NotFound("Lesson not found");
            }
            return Ok(lessonDetail);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving lesson detail: {ex.Message}");
        }
    }
}