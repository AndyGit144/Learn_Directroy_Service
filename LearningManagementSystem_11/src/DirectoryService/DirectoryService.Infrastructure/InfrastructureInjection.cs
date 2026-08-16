using DirectoryService.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure
{
    /// <summary>
    /// Точка регистрации зависимостей слоя Infrastructure в Dependency Injection.
    /// Согласно задаче 11 этот метод должен:
    /// 1. Обязательно регистрировать DbContext.
    /// 2. Обязательно регистрировать PostgreSqlConnectionOptions и биндить к нему конфигурацию.
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

            return services;
        }
    }
}
