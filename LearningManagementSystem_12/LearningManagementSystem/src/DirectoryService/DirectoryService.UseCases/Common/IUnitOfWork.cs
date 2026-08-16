namespace DirectoryService.UseCases.Common
{
    /// <summary>
    /// Паттерн "единица работы" (Unit Of Work). Задача 12: вместо того, чтобы каждый
    /// метод репозитория сам вызывал SaveChanges (что порождает лишние отправки SQL
    /// и не даёт гарантий целостности при нескольких операциях подряд), обработчики
    /// команд накапливают изменения через репозитории (они лишь помечают сущности
    /// в ChangeTracker'е EF Core), а затем ОДИН раз подтверждают их вызовом
    /// <see cref="SaveChangesAsync"/>.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Отправляет накопленные в ChangeTracker'e изменения во внешнее хранилище.
        /// </summary>
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
