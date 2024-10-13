using MediatR;
using SimpleTaskManagement.Common.Models.Queries;

namespace SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTasks;

public class GetTasksQuery : IRequest<List<GetTasksViewModel>>
{
}