namespace Domain.Common
{
    /// <summary>
    /// Тип ошибки бизнес-логики (задача 13). Используется для маппинга
    /// в HTTP-статус код на уровне Envelope (WebApi слой).
    /// </summary>
    public enum ErrorType
    {
        Validation,
        Conflict,
        NotFound,
        InternalError,
        InvalidFormat,
        None
    }
}
