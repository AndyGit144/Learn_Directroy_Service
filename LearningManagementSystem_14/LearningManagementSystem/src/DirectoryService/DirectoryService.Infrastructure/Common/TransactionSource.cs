using DirectoryService.UseCases.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace DirectoryService.Infrastructure.Common
{
    /// <summary>
    /// Фабрика, создающая транзакцию для текущего <see cref="LmsDbContext"/>
    /// (задача 12, п.5 "Менеджер (фабрика) транзакции").
    /// </summary>
    public sealed class TransactionSource : ITransactionSource
    {
        private readonly LmsDbContext _context;

        public TransactionSource(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<ITransactionScope> BeginTransactionScopeAsync(CancellationToken ct = default)
        {
            IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(ct);
            return new TransactionScope(transaction);
        }
    }
}
