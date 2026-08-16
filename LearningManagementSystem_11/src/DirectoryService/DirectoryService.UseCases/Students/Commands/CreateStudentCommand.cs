namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Команда "Создать ученика".
    /// </summary>
    public sealed record CreateStudentCommand(
        string FirstName,
        string LastName,
        int Age,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);
}
