using Domain.Entities;
using Domain.Value_Objects;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Создать ученика" (задача 13: работа с ошибками через
    /// Result паттерн вместо Exception). Репозиторий только помечает агрегат
    /// в ChangeTracker'e (AddAsync), а реальная отправка SQL происходит один раз —
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

        public async Task<Result<Student, Error>> Handle(CreateStudentCommand command, CancellationToken ct = default)
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
                // Доменные исключения валидации ценностных объектов конкретизируем
                // в ожидаемую ошибку Result-а — это не аварийная ситуация,
                // а обычное нарушение бизнес-правила входных данных.
                return Failure<Student>(Error.Validation(ex.Message));
            }

            // Проверка уникальности email — аналог AnyAsync() из задачи 12,
            // "Проверка на существование (EXISTS)".
            if (await _repository.ExistsByEmailAsync(email, ct))
                return Failure<Student>(Error.Conflict($"Ученик с email '{email.Value}' уже зарегистрирован."));

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
                return Failure<Student>(Error.Validation(ex.Message));
            }

            // AddAsync только помечает Student как Added в ChangeTracker'e —
            // запись в таблицу students ещё не произошла.
            await _repository.AddAsync(student, ct);

            // А вот тут уже идёт отправка SQL (INSERT).
            Result<Nothing, Error> saving = await _unitOfWork.SaveChangesAsync(ct);
            if (saving.IsFailure)
                return Failure<Student>(saving.OnError);

            return Success<Student, Error>(student);
        }
    }
}
