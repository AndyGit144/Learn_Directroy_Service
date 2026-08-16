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
    }
}
