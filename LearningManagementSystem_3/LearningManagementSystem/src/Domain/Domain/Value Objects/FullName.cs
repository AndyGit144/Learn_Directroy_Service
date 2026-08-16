using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Value_Objects
{
    public record FullName
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string MiddleName { get; }
        public string FullNameString => $"{LastName} {FirstName} {MiddleName}";
        public string ShortName => $"{LastName} {FirstName[0]}.{MiddleName[0]}.";

        private FullName(string firstName, string lastName, string middleName)
        {
            FirstName = firstName;
            LastName = lastName;
            MiddleName = middleName;
        }

        public static FullName Create(string firstName, string lastName, string middleName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("Имя не может быть пустым", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Фамилия не может быть пустой", nameof(lastName));

            if (string.IsNullOrWhiteSpace(middleName))
                throw new ArgumentException("Отчество не может быть пустым", nameof(middleName));

            ValidateName(firstName, nameof(firstName));
            ValidateName(lastName, nameof(lastName));
            ValidateName(middleName, nameof(middleName));

            return new FullName(
                firstName.Trim(),
                lastName.Trim(),
                middleName.Trim()
            );
        }

        private static void ValidateName(string name, string paramName)
        {
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != '-' && c != ' ')
                    throw new ArgumentException(
                        $"{paramName} может содержать только буквы, дефис и пробел",
                        paramName);
            }
        }
    }
}
