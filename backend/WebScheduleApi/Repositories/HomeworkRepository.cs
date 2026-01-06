using System.Collections.Concurrent;

namespace WebScheduleApi.Repositories;

public class HomeworkRepository : IHomeworkRepository
{
    private readonly ConcurrentDictionary<string, string> _homeworkData = new();

    public Task<string> GetHomeworkAsync(string date, int order)
    {
        if (string.IsNullOrEmpty(date))
        {
            throw new ArgumentNullException(nameof(date));
        }
        var key = $"{date}_{order}";
        _homeworkData.TryGetValue(key, out var homework);
        return Task.FromResult(homework ?? string.Empty);
    }

    public Task SaveHomeworkAsync(string date, int order, string homework)
    {
        if (string.IsNullOrEmpty(date))
        {
            throw new ArgumentNullException(nameof(date));
        }
        var key = $"{date}_{order}";
        _homeworkData[key] = homework;
        return Task.CompletedTask;
    }
}