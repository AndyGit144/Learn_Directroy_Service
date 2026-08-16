using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Value_Objects
{
        public record ClassName
        {
            public short Grade { get; }
            public char Letter { get; }
            public string Value => $"{Grade}{Letter}";

            private ClassName(short grade, char letter)
            {
                Grade = grade;
                Letter = letter;
            }

            public static ClassName Create(short grade, char letter)
            {
                if (grade < 1 || grade > 11)
                    throw new ArgumentException("Параллель должна быть от 1 до 11", nameof(grade));

                letter = char.ToUpper(letter);

                if (!char.IsLetter(letter) || letter < 'А' || letter > 'Я')
                    throw new ArgumentException("Литера должна быть буквой кириллицы от А до Я", nameof(letter));

                return new ClassName(grade, letter);
            }

            public static ClassName Parse(string value)
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
                    throw new ArgumentException("Некорректный формат названия класса", nameof(value));

                string gradeStr = value[..^1]; // Все кроме последнего символа
                char letter = value[^1]; // Последний символ

                if (!short.TryParse(gradeStr, out short grade))
                    throw new ArgumentException("Не удалось распознать параллель класса", nameof(value));

                return Create(grade, letter);
            }
        }
    }
