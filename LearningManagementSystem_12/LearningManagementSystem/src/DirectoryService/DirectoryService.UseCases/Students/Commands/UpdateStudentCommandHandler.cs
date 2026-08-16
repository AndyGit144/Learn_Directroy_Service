using Domain.Entities;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Обновить контактные данные ученика".
    /// Демонстрирует ключевое преимущество ChangeTracker'а (задача 12, п.3):
    /// сущность получена из репозитория (а значит уже отслеживается контекстом),
    /// метод домена меняет её поля, и после этого достаточно один раз вызвать
    /// SaveChangesAsync() — вручную репозиторий Update() вызывать не нужно,
    /// EF Core сам построит нужный UPDATE запрос по разнице состояний.
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

        public async Task<Student> Handle(UpdateStudentCommand command, CancellationToken ct = default)
        {
            Student? student = await _repository.GetByIdAsync(command.Id, ct: ct);
            if (student is null)
                throw new UseCaseNotFoundException($"Не найден ученик с ID: {command.Id}");

            try
            {
                Domain.Entities.Email email = Domain.Entities.Email.Create(command.Email);
                Domain.Entities.PhoneNumber phone = Domain.Entities.PhoneNumber.Create(command.ParentPhone);

                // Меняем состояние отслеживаемого агрегата — ChangeTracker сам
                // пометит его как Modified.
                student.UpdateContactInfo(email, phone);
                student.SetSpecialNeeds(command.HasSpecialNeeds);
            }
            catch (ArgumentException ex)
            {
                throw new UseCaseValidationException(ex.Message);
            }

            await _unitOfWork.SaveChangesAsync(ct);

            return student;
        }
    }
}
