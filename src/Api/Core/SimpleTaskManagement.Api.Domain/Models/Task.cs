namespace SimpleTaskManagement.Api.Domain.Models;

public class Task : BaseEntity
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime DueDate { get; set; }

    public int? ParentId { get; set; }

    public List<int> Dependencies { get; set; } = new List<int>();
}