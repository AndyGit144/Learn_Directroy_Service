using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Validation;
using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Удалить ученика" (задача 14: валидация через
    /// <see cref="DeleteStudentCommandValidator"/>).
    /// </summary>
    public sealed class DeleteStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteStudentCommand> _validator;

        public DeleteStudentCommandHandler(
            IStudentsRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<DeleteStudentCommand> validator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<Nothing, Error>> Handle(DeleteStudentCommand command, CancellationToken ct = default)
        {
            Result<DeleteStudentCommand, Error> validation = await _validator.ValidateToResultAsync(command, ct);
            if (validation.IsFailure)
                return Failure<Nothing>(validation.OnError);

            var student = await _repository.GetByIdAsync(command.StudentId, ct: ct);
            if (student is null)
                return Failure<Nothing>(Error.NotFound($"Не найден ученик с ID: {command.StudentId}"));

            // Только помечает Student как Deleted в ChangeTracker'e.
            _repository.Remove(student);

            // А вот тут уже идёт отправка SQL (DELETE).
            return await _unitOfWork.SaveChangesAsync(ct);
        }
    }
}
