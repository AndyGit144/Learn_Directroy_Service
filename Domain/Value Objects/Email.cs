using System;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public record Email
    {
        public const int MAX_EMAIL_LENGTH = 100;
        public const int MIN_EMAIL_LENGTH = 5;

        // Регулярное выражение для проверки формата email:
        // пользовательская часть @ доменная часть . зона (2 и более букв)
        private static readonly Regex _emailValidationRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        public static Email Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Почта не может быть пустой", nameof(value));

            value = value.Trim();

            if (value.Length > MAX_EMAIL_LENGTH)
                throw new ArgumentException($"Почта превышает длину {MAX_EMAIL_LENGTH}", nameof(value));

            if (value.Length < MIN_EMAIL_LENGTH)
                throw new ArgumentException($"Почта менее длины {MIN_EMAIL_LENGTH}", nameof(value));

            Match match = _emailValidationRegex.Match(value);
            if (!match.Success)
                throw new ArgumentException("Почта некорректного формата", nameof(value));

            return new Email(value);
        }
    }
}
