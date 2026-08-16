# Задача 14. FluentValidation

## 1. Что добавлено

- Пакеты: `FluentValidation` (в `DirectoryService.UseCases`) и
  `FluentValidation.DependencyInjectionExtensions` (в `DirectoryService.WebApi`,
  для `AddValidatorsFromAssemblyContaining<T>()`).
- **Валидаторы** для всех входных моделей Application-слоя (`Students`):
  - `CreateStudentCommandValidator`
  - `UpdateStudentCommandValidator`
  - `ChangeStudentClassCommandValidator`
  - `DeleteStudentCommandValidator`
  - `GetStudentByIdQueryValidator`
  (`GetStudentsQuery` — без параметров, валидатор не нужен.)
- **Адаптер домен ↔ FluentValidation** — `DirectoryService.UseCases.Validation.DomainRuleValidatorExtensions.MustSatisfyDomainRule(...)`:
  оборачивает вызов доменной фабрики (`Email.Create`, `PhoneNumber.Create`,
  `FullName.Create` и т.п., которые бросают `ArgumentException`) в
  FluentValidation `Custom`-правило, перехватывая исключение и превращая его
  в `ValidationFailure`. Это позволяет не дублировать бизнес-правила домена
  (длины строк, регулярные выражения email/телефона, допустимые символы в
  имени) отдельным набором FluentValidation-правил — валидатор просто
  переиспользует то, что уже есть в доменном слое.
- **Маппинг FluentValidation → Result** — `DirectoryService.UseCases.Validation.FluentValidationResultExtensions.ValidateToResultAsync<T>(...)`:
  прогоняет `IValidator<T>.ValidateAsync(...)`, при неуспехе формирует
  `Result<T, Error>` с `Error.Validation(message, IReadOnlyList<ValidationErrorDetail>)`,
  где `ValidationErrorDetail(PropertyName, Message)` — деталь по каждому
  нарушенному правилу сразу по всем полям (не только по первому найденному).
- `Domain.Common.Error` расширен полем `ValidationErrors` (`IReadOnlyList<ValidationErrorDetail>?`),
  заполняется только для ошибок валидации с несколькими деталями.
- `Envelope<T>` (WebApi) получил поле `Errors` — при ошибке валидации клиент
  видит не только общее сообщение, но и список конкретных нарушенных полей.
- Каждый обработчик (`CreateStudentCommandHandler` и т.д.) в самом начале
  `Handle(...)` вызывает `await _validator.ValidateToResultAsync(command, ct)`
  и, если результат неуспешен, сразу возвращает `Failure<T>(validation.OnError)`
  — до похода в репозиторий/домен/транзакцию.
- Валидаторы регистрируются одной строкой в `Program.cs`:
  ```csharp
  builder.Services.AddValidatorsFromAssemblyContaining<CreateStudentCommandValidator>();
  ```
  Обработчики получают нужный `IValidator<TCommand>` через конструктор (DI).

## 2. Пример адаптера

```csharp
// DomainRuleValidatorExtensions.cs
public static IRuleBuilderOptions<T, TProperty> MustSatisfyDomainRule<T, TProperty>(
    this IRuleBuilder<T, TProperty> ruleBuilder,
    Action<TProperty> domainFactory,
    string? propertyName = null)
{
    return ruleBuilder.Custom((value, context) =>
    {
        try { domainFactory(value); }
        catch (ArgumentException ex) { context.AddFailure(propertyName ?? context.PropertyName, ex.Message); }
    });
}
```

Использование в валидаторе:

```csharp
RuleFor(x => x.Email).MustSatisfyDomainRule(value => Email.Create(value));
RuleFor(x => x.ParentPhone).MustSatisfyDomainRule(value => PhoneNumber.Create(value));

// Правило уровня всей команды — три поля валидируются одной доменной фабрикой:
RuleFor(x => x)
    .MustSatisfyDomainRule(
        c => FullName.Create(c.FirstName, c.LastName, c.MiddleName),
        propertyName: nameof(CreateStudentCommand.FirstName));
```

## 3. Пример ответа при ошибке валидации

`POST /api/students` с пустым email и слишком коротким телефоном вернёт `400 Bad Request`:

```json
{
  "result": null,
  "error": "Входные данные не прошли валидацию.",
  "errors": [
    { "propertyName": "Email", "message": "Почта не может быть пустой" },
    { "propertyName": "ParentPhone", "message": "Номер телефона менее длины 7" }
  ]
}
```

## 4. Разграничение ответственности (не дублируем правила)

| Правило | Где живёт | Кто проверяет |
|---|---|---|
| Формат/длина email, телефона, ФИО | Domain VO (`Email.Create`, `PhoneNumber.Create`, `FullName.Create`) | Валидатор через адаптер `MustSatisfyDomainRule` |
| Возраст ученика 5–25 лет, дата рождения не в будущем | Domain (`Student.ValidateAge`, приватный) + продублирован быстрый pre-check в `CreateStudentCommandValidator` (fail fast до похода в БД) | Валидатор (UX) + домен (последняя линия защиты) |
| Соответствие возраста параллели при переводе класса | Domain (`Student.TransferToClass`) — нужен загруженный агрегат | Только домен; валидатор проверяет лишь форму (`NewClassId`/`NewGrade` не пустые/в диапазоне) |
| Уникальность email | Инфраструктура (`ExistsByEmailAsync`) | Handler, отдельно от валидатора (требует похода в БД) |

Валидатор — это быстрый, дешёвый предварительный барьер до похода в домен/БД;
доменный слой остаётся источником истины и продолжает enforced-проверять те
же правила самостоятельно (defense in depth).
