using Domain.Entities;
using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Перевести ученика в другой класс".
    /// Задача 12, п.5: операция оборачивается в транзакцию с пессимистичной
    /// блокировкой строки ученика (FOR UPDATE), чтобы защититься от гонки —
    /// если два параллельных запроса переводят одного и того же ученика,
    /// второй запрос дождётся завершения первого и увидит уже актуальное
    /// состояние агрегата, а не "устаревший снимок" таблицы.
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

        public async Task<Student> Handle(ChangeStudentClassCommand command, CancellationToken ct = default)
        {
            if (command.NewClassId == Guid.Empty)
                throw new UseCaseValidationException("Идентификатор нового класса не может быть пустым.");

            // Начинаем транзакцию.
            await using ITransactionScope scope = await _transactionSource.BeginTransactionScopeAsync(ct);

            // Блокируем строку ученика (SELECT ... FOR UPDATE) — второй параллельный
            // запрос на перевод того же ученика будет ждать, пока эта транзакция
            // не завершится (см. Task_13.pdf, п.5 "Пессимистичная блокировка").
            Student? student = await _repository.GetByIdAsync(command.StudentId, withLock: true, ct: ct);
            if (student is null)
                throw new UseCaseNotFoundException($"Не найден ученик с ID: {command.StudentId}");

            if (student.ClassId == command.NewClassId)
                throw new UseCaseValidationException("Ученик уже закреплён за этим классом.");

            try
            {
                student.TransferToClass(command.NewClassId, command.NewGrade);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                throw new UseCaseValidationException(ex.Message);
            }

            // Отправляем UPDATE в рамках открытой транзакции.
            await _unitOfWork.SaveChangesAsync(ct);

            // Подтверждаем транзакцию (при ошибке ITransactionScope сам откатит её).
            await scope.CommitAsync(ct);

            return student;
        }
    }
}
