namespace DirectoryService.UseCases.Common
{
    /// <summary>
    /// Область (сама) транзакции, которой можно управлять из Use Case слоя:
    /// подтвердить (<see cref="CommitAsync"/>) или откатить изменения.
    /// Реализуется в Infrastructure слое поверх <see cref="Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction"/>,
    /// но сам Use Case слой знает только об этой абстракции (задача 12, пункт "Транзакция").
    /// </summary>
    public interface ITransactionScope : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Подтверждает транзакцию. Если в процессе подтверждения происходит ошибка,
        /// реализация обязана откатить транзакцию перед тем, как перевыбросить исключение.
        /// </summary>
        Task CommitAsync(CancellationToken ct = default);

        /// <summary>
        /// Явно откатывает транзакцию назад.
        /// </summary>
        Task RollbackAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Фабрика (менеджер) транзакций для текущего контекста хранилища.
    /// </summary>
    public interface ITransactionSource
    {
        /// <summary>
        /// Начинает новую транзакцию и возвращает область для управления ей.
        /// </summary>
        Task<ITransactionScope> BeginTransactionScopeAsync(CancellationToken ct = default);
    }
}
