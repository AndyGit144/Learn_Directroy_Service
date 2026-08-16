namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Команда "Удалить ученика".
    /// </summary>
    public sealed record DeleteStudentCommand(Guid StudentId);
}
