namespace DirectoryService.UseCases.Students.Queries
{
    /// <summary>
    /// Запрос "Получить ученика по идентификатору".
    /// </summary>
    public sealed record GetStudentByIdQuery(Guid Id);
}
