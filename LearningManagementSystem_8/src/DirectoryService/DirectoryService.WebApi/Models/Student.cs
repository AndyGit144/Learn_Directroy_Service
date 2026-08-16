namespace DirectoryService.WebApi.Models
{
    /// <summary>
    /// Представляет сущность ученика в приложении (учебный пример, отдельный
    /// от доменного слоя — используется только для демонстрации CRUD/контроллеров).
    /// </summary>
    public sealed class Student
    {
        /// <summary>
        /// Уникальный идентификатор ученика.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя ученика.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия ученика.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Возраст ученика. Допустимый диапазон — от 5 до 25 лет.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Идентификатор класса, за которым закреплён ученик.
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// Электронная почта ученика.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Телефон родителя/опекуна.
        /// </summary>
        public string ParentPhone { get; set; } = string.Empty;

        /// <summary>
        /// Признак наличия у ученика особых образовательных потребностей.
        /// </summary>
        public bool HasSpecialNeeds { get; set; }
    }
}
