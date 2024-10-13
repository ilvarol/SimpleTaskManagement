using AutoMapper;
using MediatR;
using SimpleTaskManagement.Api.Application.Constants;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Models.Commands;

namespace SimpleTaskManagement.Api.Application.Features.Commands.Task.Create;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, int>
{
    private readonly ITaskRepository taskRepository;
    private readonly IMapper mapper;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        this.taskRepository = taskRepository;
        this.mapper = mapper;
    }

    public async Task<int> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var dbTask = mapper.Map<Domain.Models.Task>(request);

        taskRepository.Add(dbTask);

        return await System.Threading.Tasks.Task.FromResult(dbTask.Id);
    }
}