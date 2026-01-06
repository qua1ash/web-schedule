using WebScheduleApi.Models;

namespace WebScheduleApi.Repositories;

public interface ILessonDetailRepository
{
    Task<LessonDetailResponse?> GetLessonDetailAsync(string date, int order);
}