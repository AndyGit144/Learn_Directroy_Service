using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    using Domain.Enums;
    using global::Domain.Value_Objects;

    public class Student
    {
        public Guid Id { get; }
        public FullName Name { get; private set; }
        public DateTime DateOfBirth { get; }
        public Guid ClassId { get; private set; }
        public Email Email { get; private set; }
        public PhoneNumber ParentPhone { get; private set; }
        public bool HasSpecialNeeds { get; private set; }
        public DateTime EnrollmentDate { get; }
        public StudentStatus Status { get; private set; }
        public EntityLifetime Lifetime { get; private set; }

        /// <summary>
        /// Приватный конструктор без параметров, необходим EF Core для материализации
        /// сущности из базы данных (задача 11).
        /// </summary>
        private Student()
        {
            // ef core
            Id = Guid.Empty;
            DateOfBirth = default;
            EnrollmentDate = default;
            Name = null!;
            Email = null!;
            ParentPhone = null!;
            Status = null!;
            Lifetime = null!;
        }

        private Student(
            Guid id,
            FullName name,
            DateTime dateOfBirth,
            Guid classId,
            Email email,
            PhoneNumber parentPhone,
            bool hasSpecialNeeds)
        {
            Id = id;
            Name = name;
            DateOfBirth = dateOfBirth;
            ClassId = classId;
            Email = email;
            ParentPhone = parentPhone;
            HasSpecialNeeds = hasSpecialNeeds;
            EnrollmentDate = DateTime.UtcNow;
            Status = StudentStatus.Active;
            Lifetime = EntityLifetime.Create();
        }

        public static Student Create(
            FullName name,
            DateTime dateOfBirth,
            Guid classId,
            Email email,
            PhoneNumber parentPhone,
            bool hasSpecialNeeds = false)
        {
            if (classId == Guid.Empty)
                throw new ArgumentException("Ученик должен быть закреплен за классом");

            ValidateAge(dateOfBirth);

            return new Student(
                Guid.NewGuid(),
                name,
                dateOfBirth,
                classId,
                email,
                parentPhone,
                hasSpecialNeeds);
        }

        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        public void TransferToClass(Guid newClassId, short newGrade)
        {
            if (Status != StudentStatus.Active)
                throw new InvalidOperationException("Нельзя перевести неактивного ученика");

            if (newClassId == Guid.Empty)
                throw new ArgumentException("Идентификатор нового класса не может быть пустым");

            // Проверка соответствия возраста и параллели
            var age = GetAge();
            var expectedAge = newGrade + 6; // 1 класс - 7 лет

            if (Math.Abs(age - expectedAge) > 2)
                throw new InvalidOperationException(
                    $"Возраст ученика ({age} лет) не соответствует параллели класса ({newGrade})");

            ClassId = newClassId;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void UpdateContactInfo(Email newEmail, PhoneNumber newPhone)
        {
            Email = newEmail;
            ParentPhone = newPhone;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void SetSpecialNeeds(bool hasNeeds)
        {
            HasSpecialNeeds = hasNeeds;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void Expel(string reason)
        {
            if (Status == StudentStatus.Expelled)
                throw new InvalidOperationException("Ученик уже отчислен");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Необходимо указать причину отчисления");

            Status = StudentStatus.Expelled;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void TakeLeave()
        {
            if (Status != StudentStatus.Active)
                throw new InvalidOperationException("Только активный ученик может взять академический отпуск");

            Status = StudentStatus.OnLeave;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void ReturnFromLeave()
        {
            if (Status != StudentStatus.OnLeave)
                throw new InvalidOperationException("Ученик не находится в академическом отпуске");

            Status = StudentStatus.Active;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        private static void ValidateAge(DateTime dateOfBirth)
        {
            if (dateOfBirth >= DateTime.Today)
                throw new ArgumentException("Дата рождения не может быть в будущем или сегодня");

            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 5 || age > 25)
                throw new ArgumentException("Возраст ученика должен быть от 5 до 25 лет");
        }
    }
}
