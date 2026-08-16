using DirectoryService.Infrastructure.Common;
using DirectoryService.Infrastructure.Students;
using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Students;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure
{
    /// <summary>
    /// Точка регистрации зависимостей слоя Infrastructure в Dependency Injection.
    /// Согласно задаче 11 этот метод должен:
    /// 1. Обязательно регистрировать DbContext.
    /// 2. Обязательно регистрировать PostgreSqlConnectionOptions и биндить к нему конфигурацию.
    /// Задача 12 добавляет сюда же Repository, Unit Of Work и фабрику транзакций.
    /// </summary>
    public static class InfrastructureInjection
    {
        public static IServiceCollection AddPostgres(this IServiceCollection services)
        {
            // Регистрируем PostgreSqlConnectionOptions и биндим их к секции
            // "PostgreSqlConnectionOptions" в appsettings.json.
            services
                .AddOptions<PostgreSqlConnectionOptions>()
                .BindConfiguration(nameof(PostgreSqlConnectionOptions));

            // Контекст базы данных регистрируем как Scoped: один и тот же экземпляр
            // (а значит и одно и то же подключение/транзакция) должен использоваться
            // в рамках одного HTTP-запроса всеми сервисами, которым он нужен.
            services.AddScoped<LmsDbContext>();

            // Repository (задача 12, п.1-2): адаптер IStudentsRepository на EF Core.
            services.AddScoped<IStudentsRepository, StudentsRepository>();

            // Unit Of Work (задача 12, п.4): один SaveChangesAsync на весь use-case.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Фабрика транзакций (задача 12, п.5).
            services.AddScoped<ITransactionSource, TransactionSource>();

            return services;
        }
    }
}
