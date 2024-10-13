using AutoMapper;
using MediatR;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Infrastructure.Exceptions;
using SimpleTaskManagement.Common.Models.Commands;
using SimpleTaskManagement.Api.Application.Constants;

namespace SimpleTaskManagement.Api.Application.Features.Commands.Task.Update;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly ITaskRepository taskRepository;
    private readonly IMapper mapper;

    public UpdateTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        this.taskRepository = taskRepository;
        this.mapper = mapper;
    }

    public Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        if(request.ParentId.HasValue && request.Id == request.ParentId.Value)
            throw new DatabaseValidationException(TaskConstants.CircularDependencyMessage);

        var dbTask = taskRepository.GetByIdWithDependencyList(request.Id);
        if (dbTask is null)
            throw new DatabaseValidationException(TaskConstants.TaskNotFoundMessage);

        ValidateNoCircularDependency(dbTask, request);

        mapper.Map(request, dbTask);

        taskRepository.Update(dbTask);

        return System.Threading.Tasks.Task.FromResult(true);
    }

    private void ValidateNoCircularDependency(Domain.Models.Task dbTask, UpdateTaskCommand request)
    {
        if (!request.ParentId.HasValue || !dbTask.Dependencies.Any())
            return;

        bool anyCircularDependency = dbTask.Dependencies.Contains(request.ParentId.Value);
        if (anyCircularDependency)
            throw new DatabaseValidationException(TaskConstants.CircularDependencyMessage);
    }
}
