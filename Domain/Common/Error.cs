namespace Domain.Common
{
    /// <summary>
    /// Одна конкретная ошибка валидации одного поля/свойства входной модели
    /// (задача 14). Используется, когда <see cref="Error"/> описывает не одну,
    /// а сразу несколько нарушенных валидатором правил.
    /// </summary>
    /// <param name="PropertyName">Имя свойства входной модели, к которому относится ошибка.</param>
    /// <param name="Message">Текст ошибки.</param>
    public sealed record ValidationErrorDetail(string PropertyName, string Message);

    /// <summary>
    /// Ошибка (Error) — представляет собой ожидаемую, "допустимую" неудачу
    /// операции (в противовес Exception, который сигнализирует об аварийной,
    /// непредвиденной ситуации). Задача 13, п.3 "Result паттерн".
    /// Конструктор закрыт — создавать ошибку можно только через статические
    /// хелпер-методы под конкретный <see cref="ErrorType"/>.
    /// </summary>
    public sealed record Error
    {
        public ErrorType Type { get; private init; }

        public string Message { get; private init; }

        /// <summary>
        /// Детали ошибок валидации по каждому свойству входной модели
        /// (задача 14). Заполняется только для <see cref="ErrorType.Validation"/>,
        /// когда ошибку сформировал FluentValidation-адаптер и нарушений
        /// могло быть несколько одновременно. Для прочих типов ошибок — null.
        /// </summary>
        public IReadOnlyList<ValidationErrorDetail>? ValidationErrors { get; private init; }

        private Error(ErrorType type, string message, IReadOnlyList<ValidationErrorDetail>? validationErrors = null)
        {
            Type = type;
            Message = message;
            ValidationErrors = validationErrors;
        }

        public static Error Validation(string message) => new(ErrorType.Validation, message);

        /// <summary>
        /// Ошибка валидации с деталями по каждому нарушенному правилу
        /// (задача 14, "Все ошибки валидации возвращаются в формате Result
        /// с деталями ошибок").
        /// </summary>
        public static Error Validation(string message, IReadOnlyList<ValidationErrorDetail> errors) =>
            new(ErrorType.Validation, message, errors);

        public static Error Conflict(string message) => new(ErrorType.Conflict, message);

        public static Error NotFound(string message) => new(ErrorType.NotFound, message);

        public static Error InternalError(string message) => new(ErrorType.InternalError, message);

        public static Error InvalidFormat(string message) => new(ErrorType.InvalidFormat, message);

        public static Error None() => new(ErrorType.None, string.Empty);
    }
}
