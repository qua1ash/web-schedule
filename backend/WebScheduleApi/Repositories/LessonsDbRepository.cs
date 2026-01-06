using Npgsql;
using WebScheduleApi.Models;

namespace WebScheduleApi.Repositories;

public class LessonsDbRepository : ILessonsRepository
{
    private readonly DatabaseService _databaseService;

    public LessonsDbRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
    }

    public async Task<List<LessonResponse>> GetLessonsForDateAsync(string date)
    {
        if (string.IsNullOrEmpty(date))
        {
            throw new ArgumentNullException(nameof(date));
        }
        if (!DateTime.TryParse(date, out var parsedDate))
        {
            throw new ArgumentException("Invalid date format");
        }

        var lessons = new List<LessonResponse>();

        // First, try to get actual lessons
        var actualQuery = @"
            SELECT al.lesson_order, al.date, al.start_time::text, al.end_time::text, s.name, t.full_name, r.name
            FROM actual_lessons al
            JOIN subjects s ON al.subject_id = s.id
            JOIN teachers t ON al.teacher_id = t.id
            JOIN rooms r ON al.room_id = r.id
            WHERE al.date = @date AND al.is_cancelled = false
            ORDER BY al.lesson_order";

        using (var reader = await _databaseService.ExecuteReaderAsync(actualQuery,
            new NpgsqlParameter("@date", parsedDate.Date)))
        {
            while (await reader.ReadAsync())
            {
                lessons.Add(new LessonResponse
                {
                    Order = reader.GetInt32(0),
                    Date = reader.GetDateTime(1).ToString("yyyy-MM-dd"),
                    Start = reader.GetString(2),
                    End = reader.GetString(3),
                    Title = reader.GetString(4),
                    Teacher = reader.GetString(5),
                    Room = reader.GetString(6)
                });
            }
        }

        // If no actual lessons, get from templates
        if (!lessons.Any())
        {
            int dayOfWeek = (int)parsedDate.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7; // Sunday is 7

            var templateQuery = @"
                SELECT st.lesson_order, @date::date, st.start_time::text, st.end_time::text, s.name, t.full_name, r.name
                FROM schedule_templates st
                JOIN subjects s ON st.subject_id = s.id
                JOIN teachers t ON st.teacher_id = t.id
                JOIN rooms r ON st.room_id = r.id
                WHERE st.day_of_week = @dayOfWeek AND st.is_active = true
                ORDER BY st.lesson_order";

            using (var reader = await _databaseService.ExecuteReaderAsync(templateQuery,
                new NpgsqlParameter("@date", parsedDate.Date),
                new NpgsqlParameter("@dayOfWeek", dayOfWeek)))
            {
                while (await reader.ReadAsync())
                {
                    lessons.Add(new LessonResponse
                    {
                        Order = reader.GetInt32(0),
                        Date = parsedDate.ToString("yyyy-MM-dd"),
                        Start = reader.GetString(2),
                        End = reader.GetString(3),
                        Title = reader.GetString(4),
                        Teacher = reader.GetString(5),
                        Room = reader.GetString(6)
                    });
                }
            }
        }

        return lessons;
    }
}