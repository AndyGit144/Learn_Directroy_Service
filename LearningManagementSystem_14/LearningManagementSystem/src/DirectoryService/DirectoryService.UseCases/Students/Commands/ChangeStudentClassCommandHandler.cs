using Domain.Entities;
using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Validation;
using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Перевести ученика в другой класс".
    /// Задача 14: форма входных данных (StudentId/NewClassId/NewGrade)
    /// проверяется <see cref="ChangeStudentClassCommandValidator"/> до
    /// открытия транзакции; соответствие возраста параллели остаётся
    /// доменным правилом внутри <c>Student.TransferToClass</c>, поскольку
    /// для его проверки нужен сам загруженный агрегат.
    /// </summary>
    public sealed class ChangeStudentClassCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionSource _transactionSource;
        private readonly IValidator<ChangeStudentClassCommand> _validator;

        public ChangeStudentClassCommandHandler(
            IStudentsRepository repository,
            IUnitOfWork unitOfWork,
            ITransactionSource transactionSource,
            IValidator<ChangeStudentClassCommand> validator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _transactionSource = transactionSource;
            _validator = validator;
        }

        public async Task<Result<Student, Error>> Handle(ChangeStudentClassCommand command, CancellationToken ct = default)
        {
            Result<ChangeStudentClassCommand, Error> validation = await _validator.ValidateToResultAsync(command, ct);
            if (validation.IsFailure)
                return Failure<Student>(validation.OnError);

            // Начинаем транзакцию. Если Result окажется Failure ниже, скоуп
            // всё равно откатит незакоммиченную транзакцию при Dispose (await using).
            await using ITransactionScope scope = await _transactionSource.BeginTransactionScopeAsync(ct);

            // Блокируем строку ученика (SELECT ... FOR UPDATE) — второй параллельный
            // запрос на перевод того же ученика будет ждать, пока эта транзакция
            // не завершится.
            Student? student = await _repository.GetByIdAsync(command.StudentId, withLock: true, ct: ct);
            if (student is null)
                return Failure<Student>(Error.NotFound($"Не найден ученик с ID: {command.StudentId}"));

            if (student.ClassId == command.NewClassId)
                return Failure<Student>(Error.Conflict("Ученик уже закреплён за этим классом."));

            try
            {
                student.TransferToClass(command.NewClassId, command.NewGrade);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                return Failure<Student>(Error.Validation(ex.Message));
            }

            // Отправляем UPDATE в рамках открытой транзакции.
            Result<Nothing, Error> saving = await _unitOfWork.SaveChangesAsync(ct);
            if (saving.IsFailure)
                return Failure<Student>(saving.OnError);

            // Подтверждаем транзакцию (при исключении во время commit транзакция
            // будет автоматически откачена внутри ITransactionScope.CommitAsync).
            await scope.CommitAsync(ct);

            return Success<Student, Error>(student);
        }
    }
}
