using Domain.Entities;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Перевести ученика в другой класс".
    /// Операция оборачивается в транзакцию с пессимистичной блокировкой строки
    /// ученика (FOR UPDATE), чтобы защититься от гонки — если два параллельных
    /// запроса переводят одного и того же ученика, второй дождётся завершения
    /// первого. Задача 13: бизнес-ошибки (не найден, уже в этом классе,
    /// нарушение возрастного правила) идут через Result; настоящий сбой самой
    /// транзакции (обрыв соединения и т.п.) остаётся исключением — его ловит
    /// глобальный ExceptionMiddleware.
    /// </summary>
    public sealed class ChangeStudentClassCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionSource _transactionSource;

        public ChangeStudentClassCommandHandler(
            IStudentsRepository repository,
            IUnitOfWork unitOfWork,
            ITransactionSource transactionSource)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _transactionSource = transactionSource;
        }

        public async Task<Result<Student, Error>> Handle(ChangeStudentClassCommand command, CancellationToken ct = default)
        {
            if (command.NewClassId == Guid.Empty)
                return Failure<Student>(Error.Validation("Идентификатор нового класса не может быть пустым."));

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
