using MediatR;

namespace SimpleTaskManagement.Common.Models.Commands;

public class CreateTaskCommand : IRequest<int>
{
    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime DueDate { get; set; }

    public int? ParentId { get; set; }
}