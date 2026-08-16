using DirectoryService.Infrastructure.Common;
using DirectoryService.WebApi;

var builder = WebApplication.CreateBuilder(args);

// === Тренировка (задача 7): работа с конфигурацией ASP.NET Core ===

IConfigurationBuilder configurationBuilder = builder.Configuration.AddJsonFile("appsettings.json");
IConfigurationRoot config = configurationBuilder.Build();

IConfigurationSection loggingSection = config.GetSection("Logging");
IConfigurationSection logLevelSection = loggingSection.GetSection("LogLevel");

string? defaultLogLevel = logLevelSection.GetSection("Default").Get<string>();
string? aspNetCoreLogLevel = logLevelSection.GetSection("Microsoft.AspNetCore").Get<string>();

Console.WriteLine(defaultLogLevel);
Console.WriteLine(aspNetCoreLogLevel);

LanguageOptions languageOptions = new();
config.GetSection(nameof(LanguageOptions)).Bind(languageOptions);

Console.WriteLine(languageOptions.ApplicationLanguage);
Console.WriteLine(languageOptions.IanaTimeZone);

// === Практика (задача 7): чтение строки подключения к PostgreSQL из конфигурации ===

PostgreSqlConnectionOptions? postgreSqlOptions = builder
    .Configuration.GetSection(nameof(PostgreSqlConnectionOptions))
    .Get<PostgreSqlConnectionOptions>();

if (postgreSqlOptions == null)
    throw new ApplicationException("Конфигурация базы данных PostgreSQL не задана.");

Console.WriteLine(postgreSqlOptions.HostName);
Console.WriteLine(postgreSqlOptions.DatabaseName);
Console.WriteLine(postgreSqlOptions.UserName);
Console.WriteLine(postgreSqlOptions.Password);

// === Задача 8: контроллеры + Swagger ===

// подключаем контроллеры
builder.Services.AddControllers();

// подключаем EndpointsApiExploler, чтобы разметить контроллеры для swagger
builder.Services.AddEndpointsApiExplorer();

// подключаем swagger, чтобы потом можно было открыть интерфейс,
// а так же аннотации (SwaggerOperation и т.д.) для документирования эндпоинтов.
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
});

var app = builder.Build();

// используем swagger, чтобы появился интерфейс.
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

// размечаем контроллеры.
app.MapControllers();

app.Run();
