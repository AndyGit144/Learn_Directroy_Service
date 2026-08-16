using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Валидатор команды "Перевести ученика в другой класс" (задача 14).
    /// Соответствие возраста ученика параллели класса — уже доменное правило
    /// внутри агрегата (<c>Student.TransferToClass</c>), для его проверки
    /// нужен сам загруженный агрегат, поэтому здесь проверяется только форма
    /// входных данных.
    /// </summary>
    public sealed class ChangeStudentClassCommandValidator : AbstractValidator<ChangeStudentClassCommand>
    {
        public ChangeStudentClassCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEqual(Guid.Empty)
                .WithMessage("Идентификатор ученика не может быть пустым.");

            RuleFor(x => x.NewClassId)
                .NotEqual(Guid.Empty)
                .WithMessage("Идентификатор нового класса не может быть пустым.");

            RuleFor(x => x.NewGrade)
                .InclusiveBetween((short)1, (short)11)
                .WithMessage("Параллель класса должна быть от 1 до 11.");
        }
    }
}
