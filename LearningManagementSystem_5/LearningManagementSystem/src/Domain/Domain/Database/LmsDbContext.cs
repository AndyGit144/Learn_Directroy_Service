using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Database
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

        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Автоматически подхватываем все классы, реализующие IEntityTypeConfiguration<TEntity>
            // (StudentEntityConfiguration, LessonEntityConfiguration, SchoolClassEntityConfiguration и т.д.)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
