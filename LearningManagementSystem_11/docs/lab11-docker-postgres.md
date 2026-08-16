# Задача 11. Подключение PostgreSQL через Docker

## 1. Поднять PostgreSQL и Adminer в Docker

Из корня репозитория (там же, где `docker-compose.yml` и `.sln`):

```bash
docker-compose -p lms -f docker-compose.yml up -d --build
```

После этого должны подняться два контейнера:
- `lms_database` — PostgreSQL, доступен на хосте по порту **5645** (внутри контейнера слушает 5545, см. `PGPORT`/`ports` в `docker-compose.yml`).
- `adminer` — веб-интерфейс для просмотра БД, доступен на `http://localhost:8080` (Система: PostgreSQL, Сервер: `database`, Пользователь: `user`, Пароль: `password`, БД: `lms_db`).

## 2. Конфигурация приложения

Строка подключения собирается из секции `PostgreSqlConnectionOptions` в
`src/DirectoryService/DirectoryService.WebApi/appsettings.json` (см.
`PostgreSqlConnectionOptions.BuildConnectionString()`), значения совпадают
с настройками контейнера `lms_database` из `docker-compose.yml`.

## 3. Установка dotnet-ef (один раз)

```bash
dotnet tool install --global dotnet-ef
```

## 4. Создание миграции

Выполняется из корня репозитория:

```bash
dotnet ef migrations add Initial \
  -p src/DirectoryService/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj \
  -s src/DirectoryService/DirectoryService.WebApi/DirectoryService.WebApi.csproj
```

Для design-time создания `LmsDbContext` (когда обычный DI-хост
приложения не поднимается) используется
`LmsDbContextFactory : IDesignTimeDbContextFactory<LmsDbContext>`
(см. `src/DirectoryService/DirectoryService.Infrastructure/Common/LmsDbContextFactory.cs`).

## 5. Применение миграции (обновление базы данных)

```bash
dotnet ef database update \
  -p src/DirectoryService/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj \
  -s src/DirectoryService/DirectoryService.WebApi/DirectoryService.WebApi.csproj
```

## Регистрация в DI

`InfrastructureInjection.AddPostgres(this IServiceCollection services)`
(в `DirectoryService.Infrastructure`) регистрирует:
1. `PostgreSqlConnectionOptions` через `AddOptions<>().BindConfiguration(...)`.
2. `LmsDbContext` как `Scoped`.

Вызывается из `Program.cs` (`DirectoryService.WebApi`):

```csharp
builder.Services.AddPostgres();
```
