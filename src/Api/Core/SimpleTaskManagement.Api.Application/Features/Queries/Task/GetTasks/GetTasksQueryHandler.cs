using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Models.Queries;
using System.Linq;

namespace SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, List<GetTasksViewModel>>
{
    private readonly ITaskRepository taskRepository;
    private readonly IMapper mapper;

    public GetTasksQueryHandler(ITaskRepository taskRepository, IMapper mapper)
    {
        this.taskRepository = taskRepository;
        this.mapper = mapper;
    }

    public Task<List<GetTasksViewModel>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        var query = taskRepository.GetAll();

        var viewTasks = mapper.Map<List<GetTasksViewModel>>(query);

        return System.Threading.Tasks.Task.FromResult(viewTasks);
    }
}
