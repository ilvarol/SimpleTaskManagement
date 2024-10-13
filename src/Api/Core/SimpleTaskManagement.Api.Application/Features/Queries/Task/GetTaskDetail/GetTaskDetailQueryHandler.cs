using AutoMapper;
using MediatR;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Models.Queries;

namespace SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTaskDetail;

public class GetTaskDetailQueryHandler : IRequestHandler<GetTaskDetailQuery, GetTaskDetailViewModel>
{
    private readonly ITaskRepository taskRepository;
    private readonly IMapper mapper;

    public GetTaskDetailQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        this.taskRepository = taskRepository;
        this.mapper = mapper;
    }

    public Task<GetTaskDetailViewModel> Handle(GetTaskDetailQuery request, CancellationToken cancellationToken)
    {
        var dbTask = taskRepository.GetById(request.Id);

        var viewTask = mapper.Map<GetTaskDetailViewModel>(dbTask);

        return System.Threading.Tasks.Task.FromResult(viewTask);
    }
}
