# Задача 12. Repository, Unit Of Work, транзакции и блокировка (Entity Framework Core)

## 1. Что добавлено

- **Repository** для доменного агрегата `Student`:
  `DirectoryService.UseCases.Students.IStudentsRepository` (абстракция) и
  `DirectoryService.Infrastructure.Students.StudentsRepository` (реализация на EF Core).
  `InMemoryStudentsRepository` (задачи 9-10) удалён — вместо него теперь настоящее
  хранилище на PostgreSQL.
- **Unit Of Work**: `IUnitOfWork` / `UnitOfWork` — все обработчики команд вызывают
  `SaveChangesAsync()` ровно один раз, после того как репозиторий(и) пометили
  изменения в ChangeTracker'e.
- **Транзакция + пессимистичная блокировка**: `ITransactionSource`/`TransactionSource`
  (фабрика) и `ITransactionScope`/`TransactionScope` (сама транзакция, с авто-откатом
  при ошибке подтверждения). Используется в `ChangeStudentClassCommandHandler` —
  перевод ученика в другой класс оборачивается в транзакцию, а строка ученика
  блокируется через `SELECT ... FOR UPDATE` (`IStudentsRepository.GetByIdAsync(id, withLock: true)`),
  чтобы защититься от гонки параллельных запросов.
- **Логирование SQL**: `LmsDbContext.OnConfiguring` теперь вызывает
  `optionsBuilder.LogTo(Console.WriteLine)`.
- Обработчики команд/запросов стали асинхронными и работают с доменным агрегатом
  `Domain.Entities.Student` напрямую (раньше — с отдельным DTO `UseCases.Students.Student`,
  который был удалён).

## 2. Пример потока для CreateStudentCommandHandler

1. `IStudentsRepository.ExistsByEmailAsync(...)` — `AnyAsync()`, проверка уникальности.
2. `Student.Create(...)` — доменная фабрика, валидация возраста/значений внутри агрегата.
3. `IStudentsRepository.AddAsync(student)` — только помечает `Added` в ChangeTracker'e.
4. `IUnitOfWork.SaveChangesAsync()` — единственный момент, когда отправляется `INSERT`.

## 3. Пример потока для ChangeStudentClassCommandHandler (транзакция + блокировка)

```
await using var scope = await transactionSource.BeginTransactionScopeAsync(ct);
var student = await repository.GetByIdAsync(id, withLock: true, ct); // SELECT ... FOR UPDATE
student.TransferToClass(newClassId, newGrade);                        // ChangeTracker: Modified
await unitOfWork.SaveChangesAsync(ct);                                 // UPDATE
await scope.CommitAsync(ct);                                           // COMMIT (или ROLLBACK при ошибке)
```

Пока транзакция не завершится, параллельный запрос на перевод того же ученика
будет ждать освобождения строки — это защищает от ситуации "гонки транзакций"
(два параллельных запроса читают один и тот же устаревший снимок и оба
завершаются "успешно").

## 4. Ограничительный индекс (уникальность email)

Уже задан декларативно в `StudentEntityConfiguration`:

```csharp
builder.HasIndex(x => x.Email).IsUnique();
```

Дополнительно, по образцу задачи 12 ("Создание собственной миграции"), такой же
индекс можно создать вручную через `migrationBuilder.Sql(...)`. Ниже — пример
такой миграции (создайте файл командой `dotnet ef migrations add`, затем при
необходимости замените автоматически сгенерированный `CreateIndex` на явный SQL):

```csharp
public partial class Students_Email_Unique_Index : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE UNIQUE INDEX IF NOT EXISTS students_email_unique
            ON students (email)
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS students_email_unique");
    }
}
```

## 5. Команды для генерации и применения миграций

Из корня репозитория (нужен `dotnet-ef`, см. `docs/lab11-docker-postgres.md`, п.3):

```bash
# первая миграция — создание таблицы students (и остальных, если появятся DbSet)
dotnet ef migrations add InitialCreate \
  -p src/DirectoryService/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj \
  -s src/DirectoryService/DirectoryService.WebApi/DirectoryService.WebApi.csproj

# применить к базе данных
dotnet ef database update \
  -p src/DirectoryService/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj \
  -s src/DirectoryService/DirectoryService.WebApi/DirectoryService.WebApi.csproj
```

> В песочнице, где готовился этот патч, не было установлено `dotnet`/`dotnet-ef`,
> поэтому папка `Migrations` не сгенерирована автоматически — выполните команды
> выше локально после `docker-compose up` (см. `docs/lab11-docker-postgres.md`).

## 6. Регистрация в DI

`InfrastructureInjection.AddPostgres(...)` (вызывается из `Program.cs`) теперь
дополнительно регистрирует:

```csharp
services.AddScoped<IStudentsRepository, StudentsRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<ITransactionSource, TransactionSource>();
```

Все — `Scoped`, как и сам `LmsDbContext`: один и тот же контекст (а значит и
транзакция) используется всеми зависимостями в рамках одного HTTP-запроса.
