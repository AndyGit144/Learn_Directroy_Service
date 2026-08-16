using DirectoryService.Infrastructure.Common;
using DirectoryService.WebApi;

var builder = WebApplication.CreateBuilder(args);

// === Тренировка: работа с конфигурацией ASP.NET Core ===

// 1. "Ручное" чтение значения из конфигурации через IConfigurationRoot.
IConfigurationBuilder configurationBuilder = builder.Configuration.AddJsonFile("appsettings.json");
IConfigurationRoot config = configurationBuilder.Build();

IConfigurationSection loggingSection = config.GetSection("Logging");
IConfigurationSection logLevelSection = loggingSection.GetSection("LogLevel");

string? defaultLogLevel = logLevelSection.GetSection("Default").Get<string>();
string? aspNetCoreLogLevel = logLevelSection.GetSection("Microsoft.AspNetCore").Get<string>();

Console.WriteLine(defaultLogLevel);
Console.WriteLine(aspNetCoreLogLevel);

// 2. Маппинг секции конфигурации в класс через Bind().
LanguageOptions languageOptions = new();
config.GetSection(nameof(LanguageOptions)).Bind(languageOptions);

Console.WriteLine(languageOptions.ApplicationLanguage);
Console.WriteLine(languageOptions.IanaTimeZone);

// 3. Маппинг секции конфигурации в класс через Get<T>().
LanguageOptions? languageOptionsViaGet = builder.Configuration
    .GetSection(nameof(LanguageOptions))
    .Get<LanguageOptions>();

Console.WriteLine(languageOptionsViaGet?.ApplicationLanguage);
Console.WriteLine(languageOptionsViaGet?.IanaTimeZone);

// === Практика: чтение строки подключения к PostgreSQL из конфигурации ===

PostgreSqlConnectionOptions? postgreSqlOptions = builder // WebApplicationBuilder
    .Configuration.GetSection(nameof(PostgreSqlConnectionOptions)) // IConfigurationSection
    .Get<PostgreSqlConnectionOptions>();

if (postgreSqlOptions == null)
    throw new ApplicationException("Конфигурация базы данных PostgreSQL не задана.");

Console.WriteLine(postgreSqlOptions.HostName);
Console.WriteLine(postgreSqlOptions.DatabaseName);
Console.WriteLine(postgreSqlOptions.UserName);
Console.WriteLine(postgreSqlOptions.Password);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
