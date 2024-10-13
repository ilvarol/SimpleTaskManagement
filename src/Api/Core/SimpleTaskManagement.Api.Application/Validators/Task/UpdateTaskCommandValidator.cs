using FluentValidation;
using SimpleTaskManagement.Api.Application.Constants;
using SimpleTaskManagement.Common.Models.Commands;

namespace SimpleTaskManagement.Api.Application.Validators.Task;
// Todo write test

public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(i => i.Id)
            .NotEqual(i => i.ParentId!.Value)
            .When(i => i.ParentId.HasValue && i.ParentId.Value > 0)
            .WithMessage(TaskConstants.TaskCannotDependOnItselfMessage);

        RuleFor(i => i.Title)
            .NotNull()
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage(TaskConstants.TitlePropertyValidationMessage);

        RuleFor(i => i.DueDate)
            .GreaterThan(i => DateTime.Now)
            .WithMessage(TaskConstants.DueDateShouldNotBeInThePast);
    }
}
