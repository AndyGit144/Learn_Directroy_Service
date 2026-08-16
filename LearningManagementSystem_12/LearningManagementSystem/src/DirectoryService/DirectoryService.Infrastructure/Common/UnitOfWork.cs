using DirectoryService.UseCases.Common;

namespace DirectoryService.Infrastructure.Common
{
    /// <summary>
    /// Реализация паттерна Unit Of Work поверх <see cref="LmsDbContext"/> (задача 12, п.4).
    /// Сгруппировывает все изменения, накопленные в ChangeTracker'e репозиториями,
    /// и отправляет их одним вызовом SaveChangesAsync().
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly LmsDbContext _context;

        public UnitOfWork(LmsDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}
