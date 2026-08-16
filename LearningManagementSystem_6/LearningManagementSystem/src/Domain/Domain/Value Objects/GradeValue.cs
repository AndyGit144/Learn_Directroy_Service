using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Value_Objects
{
    public record GradeValue
    {
        public short? NumericValue { get; }
        public bool IsAbsent { get; }
        public string DisplayValue => IsAbsent ? "н" : NumericValue.ToString()!;

        private GradeValue(short? numericValue, bool isAbsent)
        {
            NumericValue = numericValue;
            IsAbsent = isAbsent;
        }

        public static GradeValue Create(short value)
        {
            if (value < 2 || value > 5)
                throw new ArgumentException("Оценка должна быть от 2 до 5", nameof(value));

            return new GradeValue(value, false);
        }

        public static GradeValue CreateAbsent()
        {
            return new GradeValue(null, true);
        }

        public static GradeValue Parse(string value)
        {
            if (value.ToLower() == "н")
                return CreateAbsent();

            if (short.TryParse(value, out short numericValue))
                return Create(numericValue);

            throw new ArgumentException("Некорректное значение оценки", nameof(value));
        }

        public bool IsPassing => NumericValue >= 3;
    }
}
