namespace DirectoryService.Infrastructure.Common
{
    /// <summary>
    /// Настройки подключения к базе данных PostgreSQL, считываемые из конфигурации
    /// приложения (секция "PostgreSqlConnectionOptions" в appsettings.json).
    /// Название класса и его полей должно совпадать с названием JSON-ключей,
    /// иначе конфигурация не забиндится к объекту (задача 11).
    /// </summary>
    public sealed class PostgreSqlConnectionOptions
    {
        public required string HostName { get; init; }
        public required string Port { get; init; }
        public required string DatabaseName { get; init; }
        public required string UserName { get; init; }
        public required string Password { get; init; }

        /// <summary>
        /// Формирует строку подключения к базе данных на основе полей класса.
        /// </summary>
        public string BuildConnectionString()
        {
            const string template = "Host={0};Port={1};Username={2};Password={3};Database={4}";
            return string.Format(template, HostName, Port, UserName, Password, DatabaseName);
        }
    }
}
