using WebScheduleApi.Models;

namespace WebScheduleApi.Repositories;

public interface ILessonsRepository
{
    Task<List<LessonResponse>> GetLessonsForDateAsync(string date);
}