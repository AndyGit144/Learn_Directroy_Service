using Domain.Entities;
using Domain.Value_Objects;
using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Validation;
using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Создать ученика". Задача 14: перед выполнением
    /// бизнес-логики команда проходит через FluentValidation-валидатор
    /// (<see cref="CreateStudentCommandValidator"/>) — при нарушении любого
    /// правила обработчик сразу возвращает Result с деталями всех ошибок,
    /// не доходя до домена и репозитория.
    /// Задача 13: работа с ошибками через Result паттерн вместо Exception.
    /// Задача 12: репозиторий только помечает агрегат в ChangeTracker'e
    /// (AddAsync), а реальная отправка SQL происходит один раз — в момент
    /// вызова <see cref="IUnitOfWork.SaveChangesAsync"/>.
    /// </summary>
    public sealed class CreateStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateStudentCommand> _validator;

        public CreateStudentCommandHandler(
            IStudentsRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<CreateStudentCommand> validator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<Student, Error>> Handle(CreateStudentCommand command, CancellationToken ct = default)
        {
            Result<CreateStudentCommand, Error> validation = await _validator.ValidateToResultAsync(command, ct);
            if (validation.IsFailure)
                return Failure<Student>(validation.OnError);

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
                // Подстраховка на случай, если правило есть в домене, но ещё
                // не отражено в валидаторе — при нормальной работе сюда не
                // попадаем, т.к. валидатор уже покрывает эти же фабрики.
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
