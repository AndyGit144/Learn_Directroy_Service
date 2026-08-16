namespace DirectoryService.Infrastructure.Common
{
    /// <summary>
    /// Настройки подключения к базе данных PostgreSQL, считываемые из конфигурации
    /// приложения (секция "PostgreSqlConnectionOptions" в appsettings.json).
    /// </summary>
    public sealed class PostgreSqlConnectionOptions
    {
        public required string HostName { get; init; }
        public required string DatabaseName { get; init; }
        public required string UserName { get; init; }
        public required string Password { get; init; }

        /// <summary>
        /// Формирует строку подключения к базе данных на основе полей класса.
        /// </summary>
        public string BuildConnectionString()
        {
            throw new NotImplementedException();
        }
    }
}
