namespace TaskManagement.Models;

public class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Department { get; set; }

    public bool IsActive { get; set; }
}