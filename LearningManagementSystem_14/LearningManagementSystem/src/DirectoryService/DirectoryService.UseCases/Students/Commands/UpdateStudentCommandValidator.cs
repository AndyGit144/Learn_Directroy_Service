using Domain.Entities;
using DirectoryService.UseCases.Validation;
using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Валидатор команды "Обновить контактные данные ученика" (задача 14).
    /// </summary>
    public sealed class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Идентификатор ученика не может быть пустым.");

            RuleFor(x => x.Email)
                .MustSatisfyDomainRule(value => Email.Create(value));

            RuleFor(x => x.ParentPhone)
                .MustSatisfyDomainRule(value => PhoneNumber.Create(value));
        }
    }
}
