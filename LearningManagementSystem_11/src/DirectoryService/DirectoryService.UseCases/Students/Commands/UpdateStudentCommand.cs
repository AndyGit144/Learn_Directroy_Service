namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Команда "Полностью обновить данные ученика".
    /// </summary>
    public sealed record UpdateStudentCommand(
        Guid Id,
        string FirstName,
        string LastName,
        int Age,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);
}
