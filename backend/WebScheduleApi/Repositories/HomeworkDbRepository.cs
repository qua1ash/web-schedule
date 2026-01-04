using Npgsql;

namespace WebScheduleApi.Repositories;

public class HomeworkDbRepository : IHomeworkRepository
{
    private readonly DatabaseService _databaseService;

    public HomeworkDbRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<string> GetHomeworkAsync(string date, int order)
    {
        if (string.IsNullOrEmpty(date))
        {
            throw new ArgumentNullException(nameof(date));
        }
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            throw new ArgumentException("Invalid date format");
        }

        var query = "SELECT homework_text FROM homework WHERE date = @date AND lesson_order = @order";
        var result = await _databaseService.ExecuteScalarAsync(query,
            new NpgsqlParameter("@date", parsedDate.Date),
            new NpgsqlParameter("@order", order));

        return result?.ToString() ?? string.Empty;
    }

    public async Task SaveHomeworkAsync(string date, int order, string homework)
    {
        if (string.IsNullOrEmpty(date))
        {
            throw new ArgumentNullException(nameof(date));
        }
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            throw new ArgumentException("Invalid date format");
        }

        var query = @"
            INSERT INTO homework (date, lesson_order, homework_text)
            VALUES (@date, @order, @homework)
            ON CONFLICT (date, lesson_order)
            DO UPDATE SET homework_text = EXCLUDED.homework_text";

        await _databaseService.ExecuteNonQueryAsync(query,
            new NpgsqlParameter("@date", parsedDate.Date),
            new NpgsqlParameter("@order", order),
            new NpgsqlParameter("@homework", homework ?? string.Empty));
    }
}