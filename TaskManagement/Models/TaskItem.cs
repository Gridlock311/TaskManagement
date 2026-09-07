namespace TaskManagement.Models;

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int EmployeeId { get; set; }

    public TaskPriority Priority { get; set; }

    public TaskStatus Status { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public DateTime CreatedDate { get; set; }
}