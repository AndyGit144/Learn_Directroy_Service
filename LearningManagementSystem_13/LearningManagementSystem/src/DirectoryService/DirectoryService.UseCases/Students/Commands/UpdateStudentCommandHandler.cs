using Domain.Entities;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Обновить контактные данные ученика" (задача 13:
    /// Result вместо Exception). Демонстрирует ChangeTracker: сущность получена
    /// из репозитория (уже отслеживается контекстом), доменный метод меняет её
    /// поля, и после этого достаточно один раз вызвать SaveChangesAsync() —
    /// вручную репозиторий Update() вызывать не нужно.
    /// </summary>
    public sealed class UpdateStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentCommandHandler(IStudentsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Student, Error>> Handle(UpdateStudentCommand command, CancellationToken ct = default)
        {
            Student? student = await _repository.GetByIdAsync(command.Id, ct: ct);
            if (student is null)
                return Failure<Student>(Error.NotFound($"Не найден ученик с ID: {command.Id}"));

            try
            {
                Email email = Email.Create(command.Email);
                PhoneNumber phone = PhoneNumber.Create(command.ParentPhone);

                // Меняем состояние отслеживаемого агрегата — ChangeTracker сам
                // пометит его как Modified.
                student.UpdateContactInfo(email, phone);
                student.SetSpecialNeeds(command.HasSpecialNeeds);
            }
            catch (ArgumentException ex)
            {
                return Failure<Student>(Error.Validation(ex.Message));
            }

            Result<Nothing, Error> saving = await _unitOfWork.SaveChangesAsync(ct);
            if (saving.IsFailure)
                return Failure<Student>(saving.OnError);

            return Success<Student, Error>(student);
        }
    }
}
