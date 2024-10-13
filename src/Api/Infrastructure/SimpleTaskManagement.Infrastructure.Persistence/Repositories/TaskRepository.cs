using AutoMapper;
using SimpleTaskManagement.Api.Application.Constants;
using SimpleTaskManagement.Api.Application.Interfaces.Repositories;
using SimpleTaskManagement.Common.Infrastructure.Exceptions;
using Task = SimpleTaskManagement.Api.Domain.Models.Task;

namespace SimpleTaskManagement.Infrastructure.Persistence.Repositories;

public class TaskRepository : GenericRepository<Task>, ITaskRepository
{
    public TaskRepository(IMapper mapper) : base(mapper)
    {
        BeforeSaving = (task) => { PrepareAddedTask(task); CheckDbValidations(task); };
        BeforeDeleting = CheckRestrictions;
    }

    public Task GetByIdWithDependencyList(int id)
    {
        var task = GetById(id);

        if (task == null)
            return null!;

        task.Dependencies = GetDependencies(id);

        return task;
    }

    private List<int> GetDependencies(int parentId)
    {
        List<int> Subtasks;

        lock (_lock)
        {
            Subtasks = Get(x => x.ParentId == parentId).Select(x => x.Id).ToList();
        }

        var allDependencies = new List<int>(Subtasks);

        foreach (var subtask in Subtasks)
        {
            var dependencies = GetDependencies(subtask);
            allDependencies.AddRange(dependencies);
        }
        return allDependencies;

    }

    private void PrepareAddedTask(Task task)
    {
        if (task.Id == 0)
            task.CreatedDate = DateTime.Now;
    }

    private void CheckDbValidations(Task task)
    {
        if (string.IsNullOrEmpty(task.Title) || task.Title.Length > 100)
            throw new DatabaseValidationException(TaskConstants.TitlePropertyValidationMessage);

        if (task.DueDate <= DateTime.Now)
            throw new DatabaseValidationException(TaskConstants.DueDateShouldNotBeInThePast);

        if (task.ParentId.HasValue && task.ParentId.Value > 0)
        {
            var dbParent = GetById(task.ParentId.Value);
            if (dbParent is null)
                throw new DatabaseValidationException(TaskConstants.ParentTaskNotFoundMessage);
        }
    }


    private void CheckRestrictions(Task task)
    {
        var dependencies = GetByIdWithDependencyList(task.Id)?.Dependencies;
        if (dependencies?.Any() == true)
            throw new DatabaseValidationException(TaskConstants.TaskDependentOnOtherTasks);
    }
}