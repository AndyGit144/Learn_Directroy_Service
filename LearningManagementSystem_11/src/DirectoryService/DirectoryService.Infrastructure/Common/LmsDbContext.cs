using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.Common
{
    using LessonEntity = Domain.Entities.LMS.Domain.Aggregates.Lesson;

    /// <summary>
    /// Контекст базы данных приложения (PostgreSQL, через Npgsql.EntityFrameworkCore.PostgreSQL).
    /// Применяет все конфигурационные классы (<see cref="Microsoft.EntityFrameworkCore.IEntityTypeConfiguration{TEntity}"/>),
    /// объявленные в сборке Domain.
    /// </summary>
    public sealed class LmsDbContext : DbContext
    {
        public DbSet<Student> Students => Set<Student>();
        public DbSet<LessonEntity> Lessons => Set<LessonEntity>();
        public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();

        private readonly string _connectionString;

        public LmsDbContext(IOptions<PostgreSqlConnectionOptions> options)
        {
            _connectionString = options.Value.BuildConnectionString();
        }

        /// <summary>
        /// Конструктор для дизайн-тайм фабрики (см. <see cref="LmsDbContextFactory"/>),
        /// применяющей команды `dotnet ef migrations` / `dotnet ef database update`.
        /// </summary>
        internal LmsDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        /// <summary>
        /// применение конфигураций моделей для базы данных
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Автоматически подхватываем все классы, реализующие IEntityTypeConfiguration<TEntity>
            // (StudentEntityConfiguration, LessonEntityConfiguration, SchoolClassEntityConfiguration и т.д.)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
