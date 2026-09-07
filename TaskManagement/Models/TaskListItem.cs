namespace TaskManagement.Models;

public class TaskListItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public int EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public int Priority { get; set; }

    public string PriorityName { get; set; } = string.Empty;

    public int Status { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsOverdue { get; set; }
}