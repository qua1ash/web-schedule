namespace WebScheduleApi.Entities;

public class ScheduleTemplate
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int RoomId { get; set; }
    public int DayOfWeek { get; set; }
    public int LessonOrder { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsActive { get; set; } = true;

    public Subject Subject { get; set; } = null!;
    public Teacher Teacher { get; set; } = null!;
    public Room Room { get; set; } = null!;
}