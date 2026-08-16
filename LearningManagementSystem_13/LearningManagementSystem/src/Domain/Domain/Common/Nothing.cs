namespace Domain.Common
{
    /// <summary>
    /// Служебный тип-заглушка для <see cref="Result{T,U}"/>, когда успешный
    /// результат операции не несёт полезной нагрузки (например, Delete,
    /// SaveChanges) — вместо void, который нельзя указать generic-параметром.
    /// </summary>
    public readonly record struct Nothing
    {
        public static readonly Nothing Value = new();
    }
}
