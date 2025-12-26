using WebScheduleApi.Models;

namespace WebScheduleApi.Repositories;

public class LessonsRepository : ILessonsRepository
{
    private readonly List<LessonResponse> _mockLessons = new()
    {
        new LessonResponse { Order = 1, Date = "2024-12-26", Start = "08:00", End = "09:30", Title = "Математика", Teacher = "Иванов И.И.", Room = "101" },
        new LessonResponse { Order = 2, Date = "2024-12-26", Start = "09:45", End = "11:15", Title = "Физика", Teacher = "Петров П.П.", Room = "102" },
        new LessonResponse { Order = 3, Date = "2024-12-26", Start = "11:30", End = "13:00", Title = "Химия", Teacher = "Сидоров С.С.", Room = "103" },
        // Add more mock lessons as needed
    };

    public Task<List<LessonResponse>> GetLessonsForDateAsync(string date)
    {
        var lessons = _mockLessons.Where(l => l.Date == date).ToList();
        return Task.FromResult(lessons);
    }
}