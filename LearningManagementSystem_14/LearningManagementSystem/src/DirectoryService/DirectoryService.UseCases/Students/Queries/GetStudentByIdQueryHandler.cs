using Domain.Entities;
using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Validation;
using FluentValidation;

namespace DirectoryService.UseCases.Students.Queries
{
    /// <summary>
    /// Обработчик запроса "Получить ученика по идентификатору" (CQRS: Query-часть).
    /// Задача 14: валидация через <see cref="GetStudentByIdQueryValidator"/>.
    /// </summary>
    public sealed class GetStudentByIdQueryHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IValidator<GetStudentByIdQuery> _validator;

        public GetStudentByIdQueryHandler(IStudentsRepository repository, IValidator<GetStudentByIdQuery> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<Result<Student, Error>> Handle(GetStudentByIdQuery query, CancellationToken ct = default)
        {
            Result<GetStudentByIdQuery, Error> validation = await _validator.ValidateToResultAsync(query, ct);
            if (validation.IsFailure)
                return Failure<Student>(validation.OnError);

            Student? student = await _repository.GetByIdAsync(query.Id, ct: ct);

            return student is null
                ? Failure<Student>(Error.NotFound($"Не найден ученик с ID: {query.Id}"))
                : Success<Student, Error>(student);
        }
    }
}
