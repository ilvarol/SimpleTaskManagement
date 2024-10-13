using AutoMapper;
using SimpleTaskManagement.Api.Application.Constants;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Complete;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Delete;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Update;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Infrastructure.Exceptions;
using SimpleTaskManagement.Common.Models.Commands;
using Moq;
using Task = SimpleTaskManagement.Api.Domain.Models.Task;
using ThreadTask = System.Threading.Tasks.Task;
using SimpleTaskManagement.Api.Application.Mapping;
using SimpleTaskManagement.Api.Application.Features.Commands.Task.Create;
using System.Linq.Expressions;

namespace SimpleTaskManagement.Test;

public class TaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly IMapper _mapper;

    public TaskCommandHandlerTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public async ThreadTask CreateTaskCommandHandler_ShouldReturnTaskId_WhenTaskIsCreated()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test",
        };

        _taskRepositoryMock.Setup(repo => repo.Add(It.IsAny<Task>())).Callback<Task>(task =>
        {
            task.Id = 1;
        });

        var handler = new CreateTaskCommandHandler(_taskRepositoryMock.Object, _mapper);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(0, result);
        _taskRepositoryMock.Verify(repo => repo.Add(It.IsAny<Task>()), Times.Once);
    }

    [Fact]
    public async ThreadTask CreateTaskCommandHandler_ShouldThrow_WhenTaskIsInvalid()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = null
        };

        var handler = new CreateTaskCommandHandler(_taskRepositoryMock.Object, _mapper);

        _taskRepositoryMock.Setup(repo => repo.Add(It.IsAny<Task>()))
                   .Throws(new DatabaseValidationException(TaskConstants.TitlePropertyValidationMessage));

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseValidationException>(async () =>
            await handler.Handle(command, CancellationToken.None));

        _taskRepositoryMock.Verify(repo => repo.Add(It.IsAny<Task>()), Times.Once);
    }

    [Fact]
    public async ThreadTask CompleteTaskCommandHandler_ShouldCompleteTask_WhenSubtasksCompleted()
    {
        // Arrange
        // A -> B -> C
        Task taskA = ArrangeTaskWithDependencies(true, true);

        var handler = new CompleteTaskCommandHandler(_taskRepositoryMock.Object);
        var command = new CompleteTaskCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.True(taskA.IsCompleted);

        _taskRepositoryMock.Verify(repo => repo.Update(taskA), Times.Once);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async ThreadTask CompleteTaskCommandHandler_ShouldThrow_WhenAnySubtasksUncompleted(
        bool subtask1Completed, bool subtask2Completed)
    {
        // Arrange
        // A -> B -> C
        Task taskA = ArrangeTaskWithDependencies(subtask1Completed, subtask2Completed);

        var handler = new CompleteTaskCommandHandler(_taskRepositoryMock.Object);
        var command = new CompleteTaskCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<DatabaseValidationException>(async () =>
            await handler.Handle(command, CancellationToken.None));

        _taskRepositoryMock.Verify(repo => repo.Update(taskA), Times.Never);
    }

    [Fact]
    public async ThreadTask UpdateTaskCommandHandler_ShouldUpdateTask_WhenNoCircularDependency()
    {
        // Arrange
        var task = ArrangeTaskWithDependencies();

        var handler = new UpdateTaskCommandHandler(_taskRepositoryMock.Object, _mapper);
        var command = new UpdateTaskCommand
        {
            Id = 1,
            Title = "Test",
            ParentId = null
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _taskRepositoryMock.Verify(repo => repo.Update(It.IsAny<Task>()), Times.Once);
    }

    [Theory]
    [InlineData(2)] // Task B
    [InlineData(3)] // Task C
    public async ThreadTask UpdateTaskCommandHandler_ShouldThrow_WhenCircularDependencyDetected(int? parentId)
    {
        // Arrange
        // A -> B -> C
        var taskA = ArrangeTaskWithDependencies(true, true);
        _taskRepositoryMock.Setup(repo => repo.GetByIdWithDependencyList(1)).Returns(taskA);

        var handler = new UpdateTaskCommandHandler(_taskRepositoryMock.Object, _mapper);
        var command = new UpdateTaskCommand
        {
            Id = 1,
            Title = "Test",
            ParentId = parentId
        };

        // Act
        var exception = await Assert.ThrowsAsync<DatabaseValidationException>(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(TaskConstants.CircularDependencyMessage, exception.Message);
    }

    private Task ArrangeTaskWithDependencies(bool subtask1Completed = false, bool subtask2Completed = true)
    {
        var taskC = new Task
        {
            Id = 3,
            Title = "C",
            IsCompleted = subtask2Completed,
            Dependencies = new List<int>()
        };

        var taskB = new Task
        {
            Id = 2,
            Title = "B",
            IsCompleted = subtask1Completed,
            Dependencies = new List<int> { taskC.Id }
        };

        var taskA = new Task
        {
            Id = 1,
            Title = "A",
            IsCompleted = false,
            Dependencies = new List<int> { taskB.Id, taskC.Id }
        };

        _taskRepositoryMock.Setup(repo => repo.GetByIdWithDependencyList(1)).Returns(taskA);
        _taskRepositoryMock.Setup(repo => repo.GetByIdWithDependencyList(2)).Returns(taskB);
        _taskRepositoryMock.Setup(repo => repo.GetByIdWithDependencyList(3)).Returns(taskC);

        var tasks = new List<Task>() { taskA, taskB, taskC };

        _taskRepositoryMock.Setup(repo => repo.Get(It.IsAny<Expression<Func<Task, bool>>>()))
                   .Returns((Expression<Func<Task, bool>> predicate) => tasks.Where(predicate.Compile()).AsQueryable());

        return taskA;
    }

    [Fact]
    public async ThreadTask DeleteTaskCommandHandler_ShouldDeleteTask_WhenNoDependencies()
    {
        // Arrange
        var taskA = new Task
        {
            Id = 1,
            Title = "Task A",
            IsCompleted = false
        };

        _taskRepositoryMock.Setup(repo => repo.GetById(1)).Returns(taskA);

        var handler = new DeleteTaskCommandHandler(_taskRepositoryMock.Object, _mapper);
        var command = new DeleteTaskCommand(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _taskRepositoryMock.Verify(repo => repo.GetById(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async ThreadTask DeleteTaskCommandHandler_ShouldThrow_WhenDependencies()
    {
        // Arrange
        var taskA = new Task
        {
            Id = 1,
            Title = "Task A",
            IsCompleted = false
        };

        var taskB = new Task
        {
            Id = 2,
            Title = "Task A",
            IsCompleted = false,
            ParentId = taskA.Id
        };

        _taskRepositoryMock.Setup(repo => repo.GetById(1)).Returns(taskA);
        _taskRepositoryMock.Setup(repo => repo.GetById(2)).Returns(taskB);
        _taskRepositoryMock.Setup(repo => repo.Delete(1)).Throws(new DatabaseValidationException(TaskConstants.TaskDependentOnOtherTasks));

        var handler = new DeleteTaskCommandHandler(_taskRepositoryMock.Object, _mapper);
        var command = new DeleteTaskCommand(1);

        // Act
        var exception = await Assert.ThrowsAsync<DatabaseValidationException>(() => handler.Handle(command, CancellationToken.None));

        // Assert
        Assert.Equal(TaskConstants.TaskDependentOnOtherTasks, exception.Message);
        _taskRepositoryMock.Verify(repo => repo.GetById(It.IsAny<int>()), Times.Once);
    }

}