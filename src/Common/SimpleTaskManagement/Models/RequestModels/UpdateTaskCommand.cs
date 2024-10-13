using MediatR;

namespace SimpleTaskManagement.Common.Models.Commands;

public class UpdateTaskCommand : IRequest<bool>
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public DateTime DueDate { get; set; }

    public int? ParentId { get; set; }
}