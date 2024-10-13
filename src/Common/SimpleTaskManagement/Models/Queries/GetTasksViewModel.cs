using MediatR;

namespace SimpleTaskManagement.Common.Models.Queries;

public class GetTaskDetailViewModel : IRequest<int>
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime DueDate { get; set; }

    public int? ParentId { get; set; }
}