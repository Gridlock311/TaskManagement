namespace TaskManagement.Models;

public class DashboardStats
{
    public int TotalTask { get; set; }

    public int NewTask { get; set; }

    public int InProgressTask { get; set; }

    public int CompletedTask { get; set; }

    public int OverdueTask { get; set; }
}