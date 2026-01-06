using WebScheduleApi.Models;
using WebScheduleApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebScheduleApi.Repositories;

public class LessonDetailEfRepository : ILessonDetailRepository
{
    private readonly WebScheduleDbContext _context;

    public LessonDetailEfRepository(WebScheduleDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<LessonDetailResponse?> GetLessonDetailAsync(string date, int order)
    {
        if (string.IsNullOrEmpty(date))
        {
            throw new ArgumentNullException(nameof(date));
        }
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            throw new ArgumentException("Invalid date format");
        }

        // First, try to get actual lesson
        var actualLesson = await _context.ActualLessons
            .Where(al => al.Date.Date == parsedDate.Date && al.LessonOrder == order && !al.IsCancelled)
            .Include(al => al.Subject)
            .Include(al => al.Teacher)
            .Include(al => al.Room)
            .FirstOrDefaultAsync();

        LessonDetailResponse? response = null;

        if (actualLesson != null)
        {
            response = new LessonDetailResponse
            {
                Order = actualLesson.LessonOrder,
                Date = actualLesson.Date.ToString("yyyy-MM-dd"),
                Start = actualLesson.StartTime.ToString(@"hh\:mm"),
                End = actualLesson.EndTime.ToString(@"hh\:mm"),
                Title = actualLesson.Subject.Name,
                Teacher = actualLesson.Teacher.FullName,
                Room = actualLesson.Room.Name
            };
        }
        else
        {
            // Get from template
            int dayOfWeek = (int)parsedDate.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // Sunday is 7

            var template = await _context.ScheduleTemplates
                .Where(st => st.DayOfWeek == dayOfWeek && st.LessonOrder == order && st.IsActive)
                .Include(st => st.Subject)
                .Include(st => st.Teacher)
                .Include(st => st.Room)
                .FirstOrDefaultAsync();

            if (template != null)
            {
                response = new LessonDetailResponse
                {
                    Order = template.LessonOrder,
                    Date = parsedDate.ToString("yyyy-MM-dd"),
                    Start = template.StartTime.ToString(@"hh\:mm"),
                    End = template.EndTime.ToString(@"hh\:mm"),
                    Title = template.Subject.Name,
                    Teacher = template.Teacher.FullName,
                    Room = template.Room.Name
                };
            }
        }

        if (response != null)
        {
            // Get homework
            var homework = await _context.Homeworks
                .Where(h => h.Date.Date == parsedDate.Date && h.LessonOrder == order)
                .FirstOrDefaultAsync();

            response.Homework = homework?.HomeworkText ?? string.Empty;
        }

        return response;
    }
}