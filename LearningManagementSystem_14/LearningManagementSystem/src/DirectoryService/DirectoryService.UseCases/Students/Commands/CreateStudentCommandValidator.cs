using Domain.Entities;
using Domain.Value_Objects;
using DirectoryService.UseCases.Validation;
using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Валидатор команды "Создать ученика" (задача 14). Значения самих полей
    /// (email, телефон, ФИО) проверяются через доменный адаптер
    /// <see cref="DomainRuleValidatorExtensions.MustSatisfyDomainRule"/>,
    /// который переиспользует уже существующую бизнес-валидацию Value
    /// Object'ов доменного слоя, не дублируя её.
    /// </summary>
    public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
            RuleFor(x => x.Email)
                .MustSatisfyDomainRule(value => Email.Create(value));

            RuleFor(x => x.ParentPhone)
                .MustSatisfyDomainRule(value => PhoneNumber.Create(value));

            // Правило уровня всей команды: три поля (имя/фамилия/отчество)
            // валидируются вместе одной доменной фабрикой FullName.Create(...).
            RuleFor(x => x)
                .MustSatisfyDomainRule(
                    command => FullName.Create(command.FirstName, command.LastName, command.MiddleName),
                    propertyName: nameof(CreateStudentCommand.FirstName));

            RuleFor(x => x.ClassId)
                .NotEqual(Guid.Empty)
                .WithMessage("Ученик должен быть закреплен за классом.");

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today)
                .WithMessage("Дата рождения не может быть в будущем или сегодня.")
                .Must(dateOfBirth => GetAge(dateOfBirth) is >= 5 and <= 25)
                .WithMessage("Возраст ученика должен быть от 5 до 25 лет.");
        }

        private static int GetAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
