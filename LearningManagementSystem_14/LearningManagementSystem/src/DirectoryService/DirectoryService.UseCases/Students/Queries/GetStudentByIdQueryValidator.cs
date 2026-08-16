using FluentValidation;

namespace DirectoryService.UseCases.Students.Queries
{
    /// <summary>
    /// Валидатор запроса "Получить ученика по идентификатору" (задача 14).
    /// </summary>
    public sealed class GetStudentByIdQueryValidator : AbstractValidator<GetStudentByIdQuery>
    {
        public GetStudentByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Идентификатор ученика не может быть пустым.");
        }
    }
}
