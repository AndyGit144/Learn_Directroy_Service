namespace DirectoryService.UseCases.Common
{
    /// <summary>
    /// Исключение, сигнализирующее о нарушении бизнес-правила (валидации)
    /// внутри Use Case (Application) слоя. Контроллер отлавливает это исключение
    /// и превращает его в HTTP 400 Bad Request, не зная деталей самой проверки.
    /// </summary>
    public sealed class UseCaseValidationException : Exception
    {
        public UseCaseValidationException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Исключение, сигнализирующее о том, что сущность не найдена внутри Use Case.
    /// Контроллер отлавливает это исключение и превращает его в HTTP 404 Not Found.
    /// </summary>
    public sealed class UseCaseNotFoundException : Exception
    {
        public UseCaseNotFoundException(string message) : base(message)
        {
        }
    }
}
