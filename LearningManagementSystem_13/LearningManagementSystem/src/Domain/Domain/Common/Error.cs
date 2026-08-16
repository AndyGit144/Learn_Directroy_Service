namespace Domain.Common
{
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

        private Error(ErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        public static Error Validation(string message) => new(ErrorType.Validation, message);

        public static Error Conflict(string message) => new(ErrorType.Conflict, message);

        public static Error NotFound(string message) => new(ErrorType.NotFound, message);

        public static Error InternalError(string message) => new(ErrorType.InternalError, message);

        public static Error InvalidFormat(string message) => new(ErrorType.InvalidFormat, message);

        public static Error None() => new(ErrorType.None, string.Empty);
    }
}
