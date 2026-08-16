using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Удалить ученика".
    /// </summary>
    public sealed class DeleteStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;

        public DeleteStudentCommandHandler(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public void Handle(DeleteStudentCommand command)
        {
            var student = _repository.GetById(command.StudentId);
            if (student is null)
                throw new UseCaseNotFoundException($"Не найден ученик с ID: {command.StudentId}");

            _repository.Remove(command.StudentId);
        }
    }
}
