namespace DirectoryService.UseCases.Students
{
    /// <summary>
    /// Абстракция хранилища учеников. Use Case (Application) слой зависит только
    /// от этого интерфейса, а не от конкретной реализации хранилища (правило
    /// зависимостей, см. задачу 10) — реализация будет находиться либо в
    /// WebApi (in-memory, для учебных целей), либо в Infrastructure (например, EF Core/PostgreSQL).
    /// </summary>
    public interface IStudentsRepository
    {
        /// <summary>
        /// Возвращает всех учеников.
        /// </summary>
        IEnumerable<Student> GetAll();

        /// <summary>
        /// Возвращает ученика по идентификатору или null, если не найден.
        /// </summary>
        Student? GetById(Guid id);

        /// <summary>
        /// Добавляет нового ученика в хранилище.
        /// </summary>
        void Add(Student student);

        /// <summary>
        /// Удаляет ученика по идентификатору. Возвращает true, если ученик был удалён.
        /// </summary>
        bool Remove(Guid id);
    }
}
