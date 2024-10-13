using MediatR;
using SimpleTaskManagement.Common.Models.Queries;

namespace SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTaskDetail;

public class GetTaskDetailQuery : IRequest<GetTaskDetailViewModel>
{
    public GetTaskDetailQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}