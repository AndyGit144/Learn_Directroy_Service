using DirectoryService.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Common
{
    /// <summary>
    /// Реализация паттерна Unit Of Work поверх <see cref="LmsDbContext"/>.
    /// Задача 13: SaveChangesAsync больше не выбрасывает исключение при ожидаемых
    /// сбоях сохранения (например, нарушение ограничительного уникального индекса
    /// из-за гонки параллельных запросов) — вместо этого возвращает Result с
    /// <see cref="Error.Conflict"/>. Действительно непредвиденные сбои (обрыв
    /// соединения с БД и т.п.) по-прежнему всплывают как исключение и обрабатываются
    /// глобальным <c>ExceptionMiddleware</c>.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly LmsDbContext _context;

        public UnitOfWork(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Nothing, Error>> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
                return Success<Nothing, Error>(Nothing.Value);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Гонка параллельных транзакций / оптимистичная блокировка.
                return Failure<Nothing>(Error.Conflict(ex.Message));
            }
            catch (DbUpdateException ex)
            {
                // Например, нарушение ограничительного уникального индекса.
                return Failure<Nothing>(Error.Conflict(ex.InnerException?.Message ?? ex.Message));
            }
        }
    }
}
