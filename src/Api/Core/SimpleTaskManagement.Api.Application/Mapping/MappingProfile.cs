using AutoMapper;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Update;
using SimpleTaskManagement.Common.Models.Commands;
using SimpleTaskManagement.Common.Models.Queries;
using Task = SimpleTaskManagement.Api.Domain.Models.Task;

namespace SimpleTaskManagement.Api.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Task, Task>();
        CreateMap<Task, GetTaskDetailViewModel>();
        CreateMap<Task, GetTasksViewModel>();
        CreateMap<CreateTaskCommand, Task>();
        CreateMap<UpdateTaskCommand, Task>();
    }
}
