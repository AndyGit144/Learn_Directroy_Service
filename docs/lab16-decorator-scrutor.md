# Лабораторная 16 — Decorator + Scrutor

## Что изменено

В проекте выделена сквозная логика и вынесена из бизнес-обработчиков в generic-декораторы:

- `ValidationCommandHandlerDecorator<TCommand, TResponse>`
- `ValidationQueryHandlerDecorator<TQuery, TResponse>`
- `LoggingCommandHandlerDecorator<TCommand, TResponse>`
- `LoggingQueryHandlerDecorator<TQuery, TResponse>`
- `UnitOfWorkCommandHandlerDecorator<TCommand, TResponse>`
- `TransactionalCommandHandlerDecorator<TCommand, TResponse>`

## Новые абстракции

Добавлены единые интерфейсы обработчиков:

- `IHandler<TRequest, TResponse>`
- `ICommand<TResponse>` / `IQuery<TResponse>`
- `ICommandHandler<TCommand, TResponse>`
- `IQueryHandler<TQuery, TResponse>`

Все обработчики теперь возвращают `Result<TResponse, Error>`, поэтому декораторы одинаково работают как с успехом, так и с ожидаемыми бизнес-ошибками.

## Регистрация через Scrutor

В `DirectoryService.WebApi/UseCasesInjection.cs` настроено автоматическое сканирование обработчиков и декорирование в правильном порядке.

Порядок для **команд**:

1. `TransactionalCommandHandlerDecorator`
2. `UnitOfWorkCommandHandlerDecorator`
3. `LoggingCommandHandlerDecorator`
4. `ValidationCommandHandlerDecorator`
5. основной `*Handler`

Порядок для **запросов**:

1. `LoggingQueryHandlerDecorator`
2. `ValidationQueryHandlerDecorator`
3. основной `*Handler`

## TransactionalHandler

Для сценариев, где нужна явная транзакция, добавлен атрибут `TransactionalHandlerAttribute`.

Пример:

```csharp
[TransactionalHandler]
public sealed class ChangeStudentClassCommandHandler : ICommandHandler<ChangeStudentClassCommand, Student>
{
}
```

Декоратор сам проверяет наличие этого атрибута и открывает транзакцию только для соответствующих обработчиков.

## Что стало лучше

- бизнес-обработчики больше не содержат кода валидации, логирования и сохранения изменений;
- transaction/unit of work/validation/logging применяются единообразно ко всем обработчикам;
- новые use-case обработчики можно добавлять без ручной регистрации в `Program.cs`;
- контроллеры зависят от контрактов `ICommandHandler<,>` / `IQueryHandler<,>`, а не от конкретных классов.

## Что проверить вручную

1. `POST /api/students`
   - логирование старта/успеха команды;
   - валидация некорректного email/телефона;
   - сохранение через `UnitOfWork`.

2. `PATCH /api/students/{id}/class`
   - открытие транзакции только для обработчика, помеченного `[TransactionalHandler]`;
   - блокировка строки при `withLock: true`;
   - commit только при успешном результате.

3. `GET /api/students/{id}`
   - валидация `Guid.Empty`;
   - отсутствие транзакции и `SaveChanges` для query.
