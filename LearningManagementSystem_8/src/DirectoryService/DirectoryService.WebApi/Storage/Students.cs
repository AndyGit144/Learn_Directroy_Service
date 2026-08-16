using DirectoryService.WebApi.Models;

namespace DirectoryService.WebApi.Storage
{
    /// <summary>
    /// Хранилище учеников (имитация базы данных в виде статического словаря).
    /// </summary>
    public static class Students
    {
        /// <summary>
        /// Словарь с учениками (уже заготовленными).
        /// </summary>
        private static readonly Dictionary<Guid, Student> Storage = new()
        {
            {
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                new Student
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    FirstName = "Иван",
                    LastName = "Иванов",
                    Age = 16,
                    ClassId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Email = "ivanov@example.com",
                    ParentPhone = "+79990000001",
                    HasSpecialNeeds = false,
                }
            },
            {
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                new Student
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    FirstName = "Мария",
                    LastName = "Петрова",
                    Age = 15,
                    ClassId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Email = "petrova@example.com",
                    ParentPhone = "+79990000002",
                    HasSpecialNeeds = false,
                }
            },
            {
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                new Student
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    FirstName = "Пётр",
                    LastName = "Сидоров",
                    Age = 17,
                    ClassId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Email = "sidorov@example.com",
                    ParentPhone = "+79990000003",
                    HasSpecialNeeds = true,
                }
            },
        };

        /// <summary>
        /// Возвращает всех учеников.
        /// </summary>
        public static IEnumerable<Student> GetAll() => Storage.Values;

        /// <summary>
        /// Возвращает ученика по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор ученика.</param>
        /// <returns>Ученик или null, если не найден.</returns>
        public static Student? GetById(Guid id) => Storage.GetValueOrDefault(id);

        /// <summary>
        /// Добавляет нового ученика в хранилище.
        /// </summary>
        /// <param name="student">Ученик для добавления.</param>
        public static void Add(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            Storage[student.Id] = student;
        }

        /// <summary>
        /// Удаляет ученика по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор ученика.</param>
        /// <returns>True, если ученик был удалён; иначе false.</returns>
        public static bool Remove(Guid id) => Storage.Remove(id);
    }
}
