namespace DirectoryService.WebApi.Common
{
    /// <summary>
    /// Унифицированная обёртка результата выполнения запроса.
    /// Используется, чтобы все эндпоинты возвращали ответ в едином формате.
    /// </summary>
    /// <typeparam name="T">Тип полезной нагрузки (результата) ответа.</typeparam>
    public sealed class Envelope<T>
    {
        /// <summary>
        /// Результат выполнения операции. Заполнен, если операция прошла успешно.
        /// </summary>
        public T? Result { get; init; }

        /// <summary>
        /// Сообщение об ошибке. Заполнено, если операция завершилась неудачно.
        /// </summary>
        public string? Error { get; init; }

        /// <summary>
        /// Детали ошибок валидации по каждому нарушенному правилу (задача 14).
        /// Заполнено только когда ошибка — результат FluentValidation-валидации
        /// с несколькими одновременными нарушениями; для прочих ошибок — null.
        /// </summary>
        public IReadOnlyList<Domain.Common.ValidationErrorDetail>? Errors { get; init; }

        /// <summary>
        /// Признак успешности выполнения операции.
        /// </summary>
        public bool IsSuccess => Error is null;

        /// <summary>
        /// Создаёт успешный ответ с результатом.
        /// </summary>
        public static Envelope<T> Success(T result) => new() { Result = result };

        /// <summary>
        /// Создаёт ответ с ошибкой.
        /// </summary>
        public static Envelope<T> Failure(string error) => new() { Error = error };

        /// <summary>
        /// Создаёт ответ с ошибкой валидации и деталями по каждому свойству.
        /// </summary>
        public static Envelope<T> Failure(string error, IReadOnlyList<Domain.Common.ValidationErrorDetail>? errors) =>
            new() { Error = error, Errors = errors };
    }
}
