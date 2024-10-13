using FluentValidation;
using SimpleTaskManagement.Api.Application.Constants;
using SimpleTaskManagement.Common.Models.Commands;

namespace SimpleTaskManagement.Api.Application.Validators.Task;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
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