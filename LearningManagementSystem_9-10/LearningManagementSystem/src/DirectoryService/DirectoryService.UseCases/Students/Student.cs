namespace DirectoryService.UseCases.Students
{
    /// <summary>
    /// Ученик — сущность, с которой работает Use Case (Application) слой.
    /// Ранее находилась в WebApi (Models/Student.cs), но была перенесена сюда,
    /// т.к. слой WebApi (контроллеры) не должен владеть бизнес-сущностями —
    /// это ответственность Application/Use Case слоя (см. задачу 10).
    /// </summary>
    public sealed class Student
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int Age { get; set; }

        public Guid ClassId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string ParentPhone { get; set; } = string.Empty;

        public bool HasSpecialNeeds { get; set; }
    }
}
