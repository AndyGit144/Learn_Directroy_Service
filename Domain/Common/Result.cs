using System;

namespace Domain.Common
{
    /// <summary>
    /// Result паттерн (задача 13, п.3): обёртка, позволяющая работать как
    /// с успешным результатом операции, так и с ошибкой, не прибегая
    /// к Exception для ожидаемых ("допустимых") ошибочных ситуаций
    /// (валидация, конфликт, "не найдено" и т.д.).
    /// </summary>
    /// <typeparam name="T">Тип успешного результата.</typeparam>
    /// <typeparam name="U">Тип ошибки (как правило, <see cref="Error"/>).</typeparam>
    public sealed class Result<T, U>
    {
        /// <summary>
        /// Успех.
        /// </summary>
        private readonly T? _onSuccess;

        /// <summary>
        /// Ошибка.
        /// </summary>
        private readonly U? _onError;

        /// <summary>
        /// Получение успеха.
        /// </summary>
        /// <exception cref="InvalidOperationException">При доступе к успеху, в случае ошибки.</exception>
        public T OnSuccess => IsSuccess
            ? _onSuccess!
            : throw new InvalidOperationException("Result is failure.");

        /// <summary>
        /// Получение ошибки.
        /// </summary>
        /// <exception cref="InvalidOperationException">При доступе к ошибке, в случае успеха.</exception>
        public U OnError => IsSuccess
            ? throw new InvalidOperationException("Result is success.")
            : _onError!;

        /// <summary>
        /// Признак успеха.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Признак ошибки.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Успех.
        /// </summary>
        /// <param name="onSuccess">Что отдавать при успехе.</param>
        private Result(T onSuccess)
        {
            _onSuccess = onSuccess;
            IsSuccess = true;
        }

        /// <summary>
        /// Ошибка.
        /// </summary>
        /// <param name="onError">Что отдавать при ошибке.</param>
        private Result(U onError)
        {
            _onError = onError;
            IsSuccess = false;
        }

        public static Result<T, U> Success(T value) => new(value);

        public static Result<T, U> Error(U value) => new(value);
    }

    /// <summary>
    /// Хелпер-методы для создания <see cref="Result{T,U}"/> без явного
    /// указания класса Result (см. глобальный статический using в .csproj) —
    /// используются как будто нативный функционал языка.
    /// </summary>
    public static class Result
    {
        public static Result<T, Error> Failure<T>(Error error) => Result<T, Error>.Error(error);

        public static Result<T, U> Success<T, U>(T onSuccess) => Result<T, U>.Success(onSuccess);
    }
}
