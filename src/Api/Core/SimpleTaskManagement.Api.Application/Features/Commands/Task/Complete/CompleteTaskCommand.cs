using MediatR;

namespace SimpleTaskManagement.Api.Application.Features.Commands.Task.Complete;

public class CompleteTaskCommand : IRequest<bool>
{
    public CompleteTaskCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }

}
