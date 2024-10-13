using AutoMapper;
using MediatR;
using SimpleTaskManagement.Api.Application.Constants;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Complete;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Infrastructure.Exceptions;

namespace SimpleTaskManagement.Api.Application.Features.Commands.Task.Delete;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly ITaskRepository taskRepository;

    public CompleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var dbTask = taskRepository.GetByIdWithDependencyList(request.Id);
        if (dbTask is null)
            throw new DatabaseValidationException(TaskConstants.TaskNotFoundMessage);

        ValidateAllSubtasksCompletedAsync(dbTask);

        dbTask.IsCompleted = true;

        taskRepository.Update(dbTask);

        return System.Threading.Tasks.Task.FromResult(true);
    }

    private void ValidateAllSubtasksCompletedAsync(Domain.Models.Task dbTask)
    {
        if (dbTask.IsCompleted)
            return;

        if (HasAnyOpenSubtaskAsync(dbTask))
            throw new DatabaseValidationException(TaskConstants.SubtasksNotCompletedMessage);
    }

    private bool HasAnyOpenSubtaskAsync(Domain.Models.Task dbTask)
    {
        if (!dbTask.Dependencies.Any())
            return false;

        var any = taskRepository.Get(x => dbTask.Dependencies.Contains(x.Id) && !x.IsCompleted).Any();

        return any;
    }
}