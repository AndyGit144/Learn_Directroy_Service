namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Команда "Обновить контактные данные ученика" (email, телефон родителя,
    /// признак особых образовательных потребностей). Смена ФИО и даты рождения
    /// доменной моделью не предусмотрена, а перевод в другой класс — отдельная
    /// команда <see cref="ChangeStudentClassCommand"/> со своими бизнес-правилами.
    /// </summary>
    public sealed record UpdateStudentCommand(
        Guid Id,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);
}
