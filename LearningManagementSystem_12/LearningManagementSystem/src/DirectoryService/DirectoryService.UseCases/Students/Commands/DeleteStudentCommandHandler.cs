using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Удалить ученика".
    /// </summary>
    public sealed class DeleteStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStudentCommandHandler(IStudentsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(DeleteStudentCommand command, CancellationToken ct = default)
        {
            var student = await _repository.GetByIdAsync(command.StudentId, ct: ct);
            if (student is null)
                throw new UseCaseNotFoundException($"Не найден ученик с ID: {command.StudentId}");

            // Только помечает Student как Deleted в ChangeTracker'e.
            _repository.Remove(student);

            // А вот тут уже идёт отправка SQL (DELETE).
            await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
