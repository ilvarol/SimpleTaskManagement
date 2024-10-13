using AutoMapper;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Complete;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Create;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Delete;
using SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTaskDetail;
using SimpleTaskManagement.Api.Application.Features.Queries.Task.GetTasks;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Api.Application.Mapping;
using SimpleTaskManagement.Common.Models.Commands;
using SimpleTaskManagement.Infrastructure.Persistence.Repositories;
using Task = System.Threading.Tasks.Task;

namespace SimpleTaskManagement.Test;

public class TaskCommandsConcurrencyTests
{
    private readonly ITaskRepository taskRepository;
    private readonly CreateTaskCommandHandler createTaskCommandHandler;
    private readonly CompleteTaskCommandHandler completeTaskCommandHandler;
    private readonly GetTasksQueryHandler getTasksQueryHandler;

    public TaskCommandsConcurrencyTests()
    {
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = config.CreateMapper();

        taskRepository = new TaskRepository(mapper);
        completeTaskCommandHandler = new CompleteTaskCommandHandler(taskRepository);
        createTaskCommandHandler = new CreateTaskCommandHandler(taskRepository, mapper);
        getTasksQueryHandler = new GetTasksQueryHandler(taskRepository, mapper);
    }

    [Fact]
    public async Task TaskCommandHandlers_ShouldWorkProperly_WhenRunConcurrently()
    {
        // Arrange
        List<Task> tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    Console.WriteLine(Guid.NewGuid().ToString());
                    var task = new CreateTaskCommand { Title = $"Task - {Guid.NewGuid().ToString()}", IsCompleted = false, DueDate = DateTime.Now.AddDays(1) };
                    var taskId = await createTaskCommandHandler.Handle(task, CancellationToken.None);

                    await completeTaskCommandHandler.Handle(new CompleteTaskCommand(taskId), CancellationToken.None);
                }));

            }
        }

        var hasException = false;

        // Act
        try
        {
            await Task.WhenAll(tasks);
        }
        catch
        {
            hasException = true;
        }

        // Assert
        Assert.False(hasException);
    }
}