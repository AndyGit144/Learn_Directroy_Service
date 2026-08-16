namespace DirectoryService.UseCases.Students.Queries
{
    /// <summary>
    /// Обработчик запроса "Получить всех учеников" (CQRS: Query-часть).
    /// </summary>
    public sealed class GetStudentsQueryHandler
    {
        private readonly IStudentsRepository _repository;

        public GetStudentsQueryHandler(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public IReadOnlyCollection<Student> Handle(GetStudentsQuery query) =>
            _repository.GetAll().ToList();
    }
}
