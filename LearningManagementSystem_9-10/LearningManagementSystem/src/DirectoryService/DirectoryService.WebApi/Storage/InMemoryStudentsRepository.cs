using DirectoryService.UseCases.Students;

namespace DirectoryService.WebApi.Storage
{
    /// <summary>
    /// In-memory реализация <see cref="IStudentsRepository"/> (имитация базы данных
    /// в виде словаря). Это адаптер внешней инфраструктуры для Use Case слоя —
    /// в будущем её можно заменить на реализацию поверх PostgreSQL/EF Core, не
    /// трогая ни контроллеры, ни обработчики команд/запросов (см. задачи 9 и 10).
    /// Регистрируется в DI как Singleton, поскольку хранит состояние на всё
    /// время жизни приложения (аналогично прежнему статическому хранилищу).
    /// </summary>
    public sealed class InMemoryStudentsRepository : IStudentsRepository
    {
        private readonly Dictionary<Guid, Student> _storage = new()
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

        public IEnumerable<Student> GetAll() => _storage.Values;

        public Student? GetById(Guid id) => _storage.GetValueOrDefault(id);

        public void Add(Student student)
        {
            ArgumentNullException.ThrowIfNull(student);

            _storage[student.Id] = student;
        }

        public bool Remove(Guid id) => _storage.Remove(id);
    }
}
