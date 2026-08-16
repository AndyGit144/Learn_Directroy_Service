using Domain.Entities;
using Domain.Value_Objects;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Создать ученика". Согласно задаче 12 работа с хранилищем
    /// идёт через Repository (<see cref="IStudentsRepository"/>) + Unit Of Work
    /// (<see cref="IUnitOfWork"/>): репозиторий только помечает агрегат в
    /// ChangeTracker'e (AddAsync), а реальная отправка SQL происходит один раз —
    /// в момент вызова <see cref="IUnitOfWork.SaveChangesAsync"/>.
    /// </summary>
    public sealed class CreateStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudentCommandHandler(IStudentsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Student> Handle(CreateStudentCommand command, CancellationToken ct = default)
        {
            FullName name;
            Email email;
            PhoneNumber phone;
            try
            {
                name = FullName.Create(command.FirstName, command.LastName, command.MiddleName);
                email = Email.Create(command.Email);
                phone = PhoneNumber.Create(command.ParentPhone);
            }
            catch (ArgumentException ex)
            {
                // Доменные исключения валидации ценностных объектов превращаем
                // в исключение Use Case слоя (подход из задачи 10 не меняется).
                throw new UseCaseValidationException(ex.Message);
            }

            // Проверка уникальности email — аналог AnyAsync() из задачи 12,
            // "Проверка на существование (EXISTS)".
            if (await _repository.ExistsByEmailAsync(email, ct))
                throw new UseCaseValidationException($"Ученик с email '{email.Value}' уже зарегистрирован.");

            Student student;
            try
            {
                student = Student.Create(
                    name,
                    command.DateOfBirth,
                    command.ClassId,
                    email,
                    phone,
                    command.HasSpecialNeeds);
            }
            catch (ArgumentException ex)
            {
                throw new UseCaseValidationException(ex.Message);
            }

            // AddAsync только помечает Student как Added в ChangeTracker'e —
            // запись в таблицу students ещё не произошла.
            await _repository.AddAsync(student, ct);

            // А вот тут уже идёт отправка SQL (INSERT).
            await _unitOfWork.SaveChangesAsync(ct);

            return student;
        }
    }
}
