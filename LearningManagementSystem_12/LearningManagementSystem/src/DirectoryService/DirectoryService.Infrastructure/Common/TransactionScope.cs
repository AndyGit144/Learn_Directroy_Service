using DirectoryService.UseCases.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace DirectoryService.Infrastructure.Common
{
    /// <summary>
    /// Область (сама) транзакции, которой можно управлять (задача 12, п.5).
    /// При ошибке во время подтверждения транзакция автоматически откатывается,
    /// чем защищает данные от неконсистентности (см. пример со счетами A и Б в лекции).
    /// </summary>
    public sealed class TransactionScope : ITransactionScope
    {
        private readonly IDbContextTransaction _transaction;

        public TransactionScope(IDbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            try
            {
                await _transaction.CommitAsync(ct);
            }
            catch
            {
                await _transaction.RollbackAsync(ct);
                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            await _transaction.RollbackAsync(ct);
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }
    }
}
