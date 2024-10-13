using MediatR;

namespace SimpleTaskManagement.Api.Application.Features.Commands.Task.Delete;

public class DeleteTaskCommand : IRequest<bool>
{
    public DeleteTaskCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}