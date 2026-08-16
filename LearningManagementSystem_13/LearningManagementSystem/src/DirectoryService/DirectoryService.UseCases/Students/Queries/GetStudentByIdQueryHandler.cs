using Domain.Entities;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Queries
{
    /// <summary>
    /// Обработчик запроса "Получить ученика по идентификатору" (CQRS: Query-часть).
    /// Задача 13: Result вместо Exception для ситуации "не найдено".
    /// </summary>
    public sealed class GetStudentByIdQueryHandler
    {
        private readonly IStudentsRepository _repository;

        public GetStudentByIdQueryHandler(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<Student, Error>> Handle(GetStudentByIdQuery query, CancellationToken ct = default)
        {
            Student? student = await _repository.GetByIdAsync(query.Id, ct: ct);

            return student is null
                ? Failure<Student>(Error.NotFound($"Не найден ученик с ID: {query.Id}"))
                : Success<Student, Error>(student);
        }
    }
}
