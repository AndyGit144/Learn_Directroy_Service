using Domain.Entities;

namespace DirectoryService.UseCases.Students
{
    /// <summary>
    /// Абстракция хранилища доменного агрегата <see cref="Student"/> (задача 12).
    /// Use Case (Application) слой зависит только от этого интерфейса, а не от
    /// конкретной СУБД или ORM — реализация (на Entity Framework Core / PostgreSQL)
    /// находится в Infrastructure слое (см. <c>StudentsRepository</c>).
    /// Методы Add/Remove только помечают агрегат в ChangeTracker'e EF Core
    /// (Added/Deleted), реальная отправка SQL происходит в момент вызова
    /// <see cref="Common.IUnitOfWork.SaveChangesAsync"/>.
    /// </summary>
    public interface IStudentsRepository
    {
        /// <summary>
        /// Возвращает всех учеников. Сущности не отслеживаются контекстом
        /// (AsNoTracking) — предполагается только чтение данных.
        /// </summary>
        Task<IReadOnlyCollection<Student>> GetAllAsync(CancellationToken ct = default);

        /// <summary>
        /// Возвращает ученика по идентификатору или null, если не найден.
        /// </summary>
        /// <param name="id">Идентификатор ученика.</param>
        /// <param name="withLock">
        /// Если true — строка блокируется в рамках текущей транзакции (SELECT ... FOR UPDATE),
        /// чтобы защититься от гонки параллельных транзакций (задача 12, пункт "Транзакция").
        /// Вызывающий код обязан находиться внутри активной транзакции.
        /// </param>
        Task<Student?> GetByIdAsync(Guid id, bool withLock = false, CancellationToken ct = default);

        /// <summary>
        /// Проверяет, существует ли уже ученик с указанным email (используется для
        /// валидации уникальности при создании — аналог AnyAsync из задачи 12).
        /// Принимает уже провалидированный Value Object, чтобы сравнение шло
        /// через тот же конвертер EF Core, что настроен для поля Email.
        /// </summary>
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default);

        /// <summary>
        /// Помечает нового ученика как Added в ChangeTracker'e.
        /// </summary>
        Task AddAsync(Student student, CancellationToken ct = default);

        /// <summary>
        /// Помечает ученика как Deleted в ChangeTracker'e.
        /// </summary>
        void Remove(Student student);
    }
}
