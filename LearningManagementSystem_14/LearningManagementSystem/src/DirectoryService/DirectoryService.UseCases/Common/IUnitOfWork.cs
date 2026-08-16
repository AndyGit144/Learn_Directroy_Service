namespace DirectoryService.UseCases.Common
{
    /// <summary>
    /// Паттерн "единица работы" (Unit Of Work). Обработчики команд накапливают
    /// изменения через репозитории (они лишь помечают сущности в ChangeTracker'е
    /// EF Core), а затем ОДИН раз подтверждают их вызовом <see cref="SaveChangesAsync"/>.
    /// Задача 13: результат оборачивается в Result — ожидаемые сбои при сохранении
    /// (например, нарушение уникального индекса) превращаются в <see cref="Error"/>
    /// вместо необработанного исключения.
    /// </summary>
    public interface IUnitOfWork
    {
        Task<Result<Nothing, Error>> SaveChangesAsync(CancellationToken ct = default);
    }
}
