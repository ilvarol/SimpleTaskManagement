using AutoMapper;
using MediatR;
using SimpleTaskManagement.Api.Application.Constants;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Infrastructure.Exceptions;

namespace SimpleTaskManagement.Api.Application.Features.Commands.Task.Delete;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository taskRepository;
    private readonly IMapper mapper;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        this.taskRepository = taskRepository;
        this.mapper = mapper;
    }

    public Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var dbTask = taskRepository.GetById(request.Id);
        if (dbTask is null)
            throw new DatabaseValidationException(TaskConstants.TaskNotFoundMessage);

        taskRepository.Delete(request.Id);

        return System.Threading.Tasks.Task.FromResult(true);
    }
}