using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleTaskManagement.Api.Application.Constants;

public class TaskConstants
{
    #region dbValidation Messages

    public const string TaskNotFoundMessage = "Task not found!";
    public const string ParentTaskNotFoundMessage = "Parent task not found!";
    public const string CircularDependencyMessage = "Task cannot depend on its own dependencies.";
    public const string SubtasksNotCompletedMessage = "Subtasks must be completed first.";
    public const string TaskCannotDependOnItselfMessage = "The task cannot depend on itself.";
    public const string TitlePropertyValidationMessage = "The Title property must be provided and should not exceed 100 characters.";
    public const string DueDateShouldNotBeInThePast = "The Due Date should not be in the past.";
    public const string TaskDependentOnOtherTasks = "Task with dependencies cannot be deleted.";

    #endregion
}
