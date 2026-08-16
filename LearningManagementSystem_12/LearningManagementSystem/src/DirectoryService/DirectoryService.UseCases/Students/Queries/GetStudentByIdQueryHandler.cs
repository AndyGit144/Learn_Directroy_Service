using Domain.Entities;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Queries
{
    /// <summary>
    /// Обработчик запроса "Получить ученика по идентификатору" (CQRS: Query-часть).
    /// </summary>
    public sealed class GetStudentByIdQueryHandler
    {
        private readonly IStudentsRepository _repository;

        public GetStudentByIdQueryHandler(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public async Task<Student> Handle(GetStudentByIdQuery query, CancellationToken ct = default)
        {
            Student? student = await _repository.GetByIdAsync(query.Id, ct: ct);
            if (student is null)
                throw new UseCaseNotFoundException($"Не найден ученик с ID: {query.Id}");

            return student;
        }
    }
}
