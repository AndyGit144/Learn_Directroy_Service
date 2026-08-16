using Domain.Entities;
using DirectoryService.Infrastructure.Common;
using DirectoryService.UseCases.Students;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.Students
{
    /// <summary>
    /// Реализация <see cref="IStudentsRepository"/> поверх Entity Framework Core
    /// (задача 12, п.2 "Паттерн Repository"). Находится в Infrastructure слое,
    /// поскольку именно этот слой адаптирует работу с внешним хранилищем для
    /// бизнес-логики приложения.
    /// </summary>
    public sealed class StudentsRepository : IStudentsRepository
    {
        private readonly LmsDbContext _context;

        public StudentsRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Student>> GetAllAsync(CancellationToken ct = default)
        {
            // AsNoTracking - отдаём данные клиенту только на чтение,
            // трекинг сущности здесь не нужен и только замедлит запрос.
            return await _context.Students
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<Student?> GetByIdAsync(Guid id, bool withLock = false, CancellationToken ct = default)
        {
            if (withLock)
                await LockStudentRowAsync(id, ct);

            // FirstOrDefaultAsync() - возвращает один элемент, или null, если не найден
            // (задача 12, "Получение одной сущности").
            return await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id, ct);
        }

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken ct = default)
        {
            // AnyAsync() генерирует SELECT EXISTS(...) - задача 12, "Проверка на существование".
            // Сравниваем целиком Value Object, чтобы EF Core применил тот же
            // конвертер (HasConversion), что настроен для колонки email.
            return await _context.Students
                .AnyAsync(s => s.Email == email, ct);
        }

        public async Task AddAsync(Student student, CancellationToken ct = default)
        {
            // Помечает Student как Added в ChangeTracker'e - в БД ничего ещё не попало,
            // отправка SQL произойдёт при SaveChangesAsync() (UnitOfWork).
            await _context.Students.AddAsync(student, ct);
        }

        public void Remove(Student student)
        {
            // Помечает Student как Deleted в ChangeTracker'e.
            _context.Students.Remove(student);
        }

        /// <summary>
        /// Блокирует строку ученика в рамках текущей транзакции (SELECT ... FOR UPDATE),
        /// задача 12, п.5 "Пессимистичная блокировка". Требует активной транзакции
        /// (см. <c>TransactionSource</c>/<c>TransactionScope</c>), иначе блокировка
        /// снимается сразу после выполнения запроса.
        /// </summary>
        private async Task LockStudentRowAsync(Guid id, CancellationToken ct)
        {
            FormattableString sql = $@"
                SELECT id
                FROM students
                WHERE id = {id}
                FOR UPDATE";

            await _context.Database.ExecuteSqlInterpolatedAsync(sql, ct);
        }
    }
}
