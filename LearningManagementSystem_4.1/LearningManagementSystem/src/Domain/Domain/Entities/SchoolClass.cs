using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    using Domain.Value_Objects;

    public class SchoolClass
    {
        public Guid Id { get; }
        public ClassName Name { get; private set; }
        public string Specialization { get; private set; }
        public Guid ClassTeacherId { get; private set; }
        public string AcademicYear { get; private set; }
        public short MaxStudents { get; private set; }
        public bool IsActive { get; private set; }
        public EntityLifetime Lifetime { get; private set; }

        // Навигационные свойства
        private List<Guid> _studentIds = new();
        public IReadOnlyCollection<Guid> StudentIds => _studentIds.AsReadOnly();

        private SchoolClass(
            Guid id,
            ClassName name,
            string specialization,
            Guid classTeacherId,
            string academicYear,
            short maxStudents)
        {
            Id = id;
            Name = name;
            Specialization = specialization;
            ClassTeacherId = classTeacherId;
            AcademicYear = academicYear;
            MaxStudents = maxStudents;
            IsActive = true;
            Lifetime = EntityLifetime.Create();
        }

        public static SchoolClass Create(
            ClassName name,
            string specialization,
            Guid classTeacherId,
            string academicYear,
            short maxStudents = 30)
        {
            if (classTeacherId == Guid.Empty)
                throw new ArgumentException("Класс должен иметь классного руководителя");

            if (string.IsNullOrWhiteSpace(specialization))
                throw new ArgumentException("Специализация не может быть пустой");

            if (maxStudents <= 0 || maxStudents > 40)
                throw new ArgumentException("Максимальное количество учеников должно быть от 1 до 40");

            ValidateAcademicYear(academicYear);

            return new SchoolClass(
                Guid.NewGuid(),
                name,
                specialization,
                classTeacherId,
                academicYear,
                maxStudents);
        }

        public void AddStudent(Guid studentId)
        {
            if (!IsActive)
                throw new InvalidOperationException("Нельзя добавить ученика в неактивный класс");

            if (_studentIds.Count >= MaxStudents)
                throw new InvalidOperationException("Достигнуто максимальное количество учеников в классе");

            if (_studentIds.Contains(studentId))
                throw new InvalidOperationException("Ученик уже есть в этом классе");

            _studentIds.Add(studentId);
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void RemoveStudent(Guid studentId)
        {
            if (!_studentIds.Contains(studentId))
                throw new InvalidOperationException("Ученик не найден в классе");

            _studentIds.Remove(studentId);
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void ChangeClassTeacher(Guid newTeacherId)
        {
            if (newTeacherId == Guid.Empty)
                throw new ArgumentException("Идентификатор учителя не может быть пустым");

            ClassTeacherId = newTeacherId;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void PromoteToNextGrade()
        {
            if (Name.Grade >= 11)
                throw new InvalidOperationException("Нельзя перевести 11 класс на следующий уровень");

            Name = ClassName.Create((short)(Name.Grade + 1), Name.Letter);

            // Обновляем учебный год
            var currentYear = int.Parse(AcademicYear.Split('-')[0]);
            AcademicYear = $"{currentYear + 1}-{currentYear + 2}";

            Lifetime = Lifetime.MarkAsUpdated();
        }

        public void Archive()
        {
            IsActive = false;
            Lifetime = Lifetime.MarkAsUpdated();
        }

        private static void ValidateAcademicYear(string academicYear)
        {
            var parts = academicYear.Split('-');
            if (parts.Length != 2)
                throw new ArgumentException("Неверный формат учебного года. Ожидается: 2025-2026");

            if (!int.TryParse(parts[0], out int startYear) || !int.TryParse(parts[1], out int endYear))
                throw new ArgumentException("Неверный формат года в учебном году");

            if (endYear != startYear + 1)
                throw new ArgumentException("Конечный год должен быть на 1 больше начального");

            if (startYear < 2000 || startYear > 2100)
                throw new ArgumentException("Год должен быть в диапазоне 2000-2100");
        }
    }
}
