namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Команда "Создать ученика".
    /// </summary>
    public sealed record CreateStudentCommand(
        string FirstName,
        string LastName,
        string MiddleName,
        DateTime DateOfBirth,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);
}
