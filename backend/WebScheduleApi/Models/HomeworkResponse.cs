namespace WebScheduleApi.Models;

public class HomeworkResponse
{
    public string Date { get; set; } = string.Empty;
    public int Order { get; set; }
    public string Homework { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? Message { get; set; }
}