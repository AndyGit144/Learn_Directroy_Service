using DirectoryService.Infrastructure;
using DirectoryService.Infrastructure.Common;
using DirectoryService.UseCases.Students;
using DirectoryService.UseCases.Students.Commands;
using DirectoryService.UseCases.Students.Queries;
using DirectoryService.WebApi;
using DirectoryService.WebApi.Storage;

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

// === Задачи 9 и 10: Dependency Injection + вынос бизнес-логики в Use Case (Application) слой ===
//
// Хранилище (адаптер внешней инфраструктуры) регистрируем как Singleton: оно хранит
// состояние (in-memory "базу данных") на всё время жизни приложения, и все обработчики
// должны работать с одним и тем же экземпляром хранилища.
builder.Services.AddSingleton<IStudentsRepository, InMemoryStudentsRepository>();

// Обработчики команд и запросов (CQRS) регистрируем как Scoped: у них нет собственного
// состояния между запросами, а на один HTTP-запрос вполне достаточно одного экземпляра
// обработчика (в отличие от Singleton, каждый новый запрос получит свежий экземпляр,
// что безопаснее, если в будущем обработчик обрастёт зависимостями с внутренним состоянием).
builder.Services.AddScoped<CreateStudentCommandHandler>();
builder.Services.AddScoped<UpdateStudentCommandHandler>();
builder.Services.AddScoped<ChangeStudentClassCommandHandler>();
builder.Services.AddScoped<DeleteStudentCommandHandler>();
builder.Services.AddScoped<GetStudentsQueryHandler>();
builder.Services.AddScoped<GetStudentByIdQueryHandler>();

// === Задача 11: подключение PostgreSQL через Docker ===
//
// Регистрируем DbContext и PostgreSqlConnectionOptions (биндится из секции
// "PostgreSqlConnectionOptions" appsettings.json). Сама база данных PostgreSQL
// поднимается через docker-compose.yml (см. корень репозитория) командой:
// docker-compose -p lms -f docker-compose.yml up -d --build
builder.Services.AddPostgres();

var app = builder.Build();

// используем swagger, чтобы появился интерфейс.
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");

// размечаем контроллеры.
app.MapControllers();

app.Run();
