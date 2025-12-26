namespace WebScheduleApi.Entities;

public class Homework
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int LessonOrder { get; set; }
    public string? HomeworkText { get; set; }
}