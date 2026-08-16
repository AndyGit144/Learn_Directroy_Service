namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Команда "Перевести ученика в другой класс".
    /// </summary>
    public sealed record ChangeStudentClassCommand(Guid StudentId, Guid NewClassId);
}
