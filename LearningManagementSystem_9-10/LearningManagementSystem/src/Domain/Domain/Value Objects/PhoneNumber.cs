using System;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public record PhoneNumber
    {
        public const int MAX_PHONE_LENGTH = 20;
        public const int MIN_PHONE_LENGTH = 7;

        // Регулярное выражение для проверки формата номера телефона.
        // Поддерживает форматы вида:
        // +7 (123) 456 78-90
        // +7 123 456 78 90
        // +7-123-456-78-90
        private static readonly Regex _phoneValidationRegex = new Regex(
            @"^\+\d{1,3}[\s-]?\(?\d{3}\)?[\s-]?\d{3}[\s-]?\d{2}[\s-]?\d{2}$",
            RegexOptions.Compiled
        );

        public string Value { get; }

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static PhoneNumber Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Номер телефона не был указан", nameof(value));

            value = value.Trim();

            if (value.Length > MAX_PHONE_LENGTH)
                throw new ArgumentException($"Номер телефона превышает длину {MAX_PHONE_LENGTH}", nameof(value));

            if (value.Length < MIN_PHONE_LENGTH)
                throw new ArgumentException($"Номер телефона менее длины {MIN_PHONE_LENGTH}", nameof(value));

            Match match = _phoneValidationRegex.Match(value);
            if (!match.Success)
                throw new ArgumentException("Номер телефона имеет некорректный формат", nameof(value));

            return new PhoneNumber(value);
        }
    }
}
