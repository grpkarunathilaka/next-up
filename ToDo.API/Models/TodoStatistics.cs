using TodoApi.Models;

public class TodoStatistics
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Pending { get; set; }
    public double CompletionRate { get; set; }
    public Dictionary<string, int> ByPriority { get; set; } = new();
    public Dictionary<string, int> ByCategory { get; set; } = new();
    public int DueToday { get; set; }
    public int Overdue { get; set; }
}