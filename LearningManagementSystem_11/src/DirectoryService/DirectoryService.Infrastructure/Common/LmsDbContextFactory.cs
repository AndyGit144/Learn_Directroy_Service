using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DirectoryService.Infrastructure.Common
{
    /// <summary>
    /// Фабрика, используемая инструментами `dotnet ef` (migrations add / database update)
    /// для создания экземпляра <see cref="LmsDbContext"/> в design-time, когда обычный
    /// хост приложения (Program.cs) не запускается, а значит DI-контейнер недоступен.
    /// Читает appsettings.json из проекта, где запускается приложение (WebApi).
    /// </summary>
    public sealed class LmsDbContextFactory : IDesignTimeDbContextFactory<LmsDbContext>
    {
        public LmsDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("../DirectoryService.WebApi/appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            PostgreSqlConnectionOptions? options = configuration
                .GetSection(nameof(PostgreSqlConnectionOptions))
                .Get<PostgreSqlConnectionOptions>();

            options ??= new PostgreSqlConnectionOptions
            {
                HostName = "localhost",
                Port = "5645",
                DatabaseName = "lms_db",
                UserName = "user",
                Password = "password",
            };

            return new LmsDbContext(options.BuildConnectionString());
        }
    }
}
