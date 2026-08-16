namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Команда "Перевести ученика в другой класс".
    /// </summary>
    /// <param name="StudentId">Идентификатор ученика.</param>
    /// <param name="NewClassId">Идентификатор нового класса.</param>
    /// <param name="NewGrade">Параллель нового класса (нужна доменной модели для проверки соответствия возраста).</param>
    public sealed record ChangeStudentClassCommand(Guid StudentId, Guid NewClassId, short NewGrade);
}
