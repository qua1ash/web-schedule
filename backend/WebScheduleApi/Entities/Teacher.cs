namespace WebScheduleApi.Entities;

public class Teacher
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;

    public ICollection<ScheduleTemplate> ScheduleTemplates { get; set; } = new List<ScheduleTemplate>();
    public ICollection<ActualLesson> ActualLessons { get; set; } = new List<ActualLesson>();
}