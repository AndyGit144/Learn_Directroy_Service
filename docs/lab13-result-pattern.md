# Задача 13. Result-паттерн, маппинг в Envelope, Exception Middleware

## 1. Что добавлено

- **Result-паттерн** — `Domain.Common`:
  - `ErrorType` (Validation, Conflict, NotFound, InternalError, InvalidFormat, None);
  - `Error` (record, закрытый конструктор, хелперы `Error.Validation(...)`, `Error.Conflict(...)`, `Error.NotFound(...)`, `Error.InternalError(...)`, `Error.InvalidFormat(...)`, `Error.None()`);
  - `Result<T, U>` — обёртка с `OnSuccess`/`OnError`/`IsSuccess`/`IsFailure` (закрытые конструкторы, доступ к "не той" стороне бросает `InvalidOperationException`);
  - статический класс `Result` (тот же namespace, отличается generic-arity от `Result<T,U>`) с хелперами `Success<T,U>(...)`/`Failure<T>(...)` — доступны без префикса `Result.` благодаря глобальному `using static` (см. п.2);
  - `Nothing` — заглушка для успешного результата без полезной нагрузки (Delete, SaveChanges).
- Каждый `.csproj` (Domain, UseCases, Infrastructure, WebApi) получил:
  ```xml
  <ItemGroup>
    <Using Include="Domain.Common" />
    <Using Include="Domain.Common.Result" Static="true" />
  </ItemGroup>
  ```
  — аналог `GlobalUsings.cs` из лекции, но через `<Using>` в самом csproj.
- `IUnitOfWork.SaveChangesAsync` теперь возвращает `Task<Result<Nothing, Error>>`: ожидаемые сбои сохранения (`DbUpdateException`, `DbUpdateConcurrencyException` — например, нарушение уникального индекса при гонке параллельных запросов) превращаются в `Error.Conflict(...)`, а не вылетают исключением.
- Обработчики команд/запросов `Students` (`Create/Update/Delete/ChangeClass/GetById`) переписаны: вместо `throw new UseCaseValidationException(...)`/`UseCaseNotFoundException` теперь `return Failure<T>(Error.Validation/NotFound/Conflict(...))`. Старые exception-классы удалены — весь ожидаемый бизнес-сбой идёт через Result.
- `GetStudentsQueryHandler` Result не использует — там просто нет ошибочных сценариев (не имеет смысла оборачивать то, что не может завершиться неудачей).
- **Маппинг Result → Envelope**: `DirectoryService.WebApi.Common.ResultToEnvelopeExtensions`
  - `Result<T, Error>.ToApiResult(successMapper)` → `200 OK` с `Envelope<Y>` при успехе, иначе статус код по `ErrorType` (`NotFound → 404`, `Conflict → 409`, `Validation/InvalidFormat → 400`, `InternalError/None → 500`).
  - `Result<Nothing, Error>.ToApiResult()` → `204 No Content` при успехе (для Delete).
  - Контроллер (`StudentsController`) больше не содержит `try/catch` — только `handler.Handle(...)` → `.ToApiResult(...)`.
- **Exception Middleware**: `DirectoryService.WebApi.Middlewares.ExceptionMiddleware` — ловит по-настоящему непредвиденные исключения (обрыв соединения с БД, баг в стороннем коде и т.п.), логирует их через `ILogger`, и возвращает клиенту единый `Envelope` с `500 Internal Server Error`, не раскрывая деталей исключения. Подключается самым первым в конвейере — `app.UseExceptionMiddleware()` (расширение `IApplicationBuilder`) сразу после `builder.Build()`, до `UseSwagger`/`MapControllers`.

## 2. Разделение ответственности: Result vs Exception

Как и в лекции — оставлены оба механизма, но каждый на своём месте:

| Ситуация | Механизм |
|---|---|
| Невалидные входные данные (`Email.Create`, `FullName.Create` и т.д.) | `Result` → `Error.Validation` |
| Ученик не найден | `Result` → `Error.NotFound` |
| Email уже занят / ученик уже в этом классе / конфликт при сохранении | `Result` → `Error.Conflict` |
| Обрыв соединения с БД, необработанный баг, сбой стороннего кода | `Exception` → ловит `ExceptionMiddleware` → `500` |
| Коммит транзакции (`ITransactionScope.CommitAsync`) | `Exception` (авто-откат внутри, затем rethrow — настоящий сбой инфраструктуры) |

## 3. Пример потока (CreateStudentCommandHandler)

```csharp
public async Task<Result<Student, Error>> Handle(CreateStudentCommand command, CancellationToken ct = default)
{
    // 1. Валидация Value Object'ов -> Error.Validation при ArgumentException
    // 2. Проверка уникальности email -> Error.Conflict
    // 3. Student.Create(...) -> Error.Validation при ArgumentException
    // 4. repository.AddAsync(student, ct)
    // 5. unitOfWork.SaveChangesAsync(ct) -> Result<Nothing, Error>, пробрасываем Failure дальше
    // 6. Success<Student, Error>(student)
}
```

В контроллере это превращается в одну строку:

```csharp
Result<Student, Error> result = await handler.Handle(command, ct);
return result.ToApiResult(ToResponse);
```
