using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Валидатор команды "Удалить ученика" (задача 14).
    /// </summary>
    public sealed class DeleteStudentCommandValidator : AbstractValidator<DeleteStudentCommand>
    {
        public DeleteStudentCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEqual(Guid.Empty)
                .WithMessage("Идентификатор ученика не может быть пустым.");
        }
    }
}
