namespace WebScheduleApi.Repositories;

public interface IHomeworkRepository
{
    Task<string> GetHomeworkAsync(string date, int order);
    Task SaveHomeworkAsync(string date, int order, string homework);
}