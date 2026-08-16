using FluentValidation;
using FluentValidation.Results;

namespace DirectoryService.UseCases.Validation
{
    /// <summary>
    /// Маппинг результата FluentValidation в Result паттерн (задача 14):
    /// "Все ошибки валидации возвращаются в формате Result с деталями ошибок".
    /// </summary>
    public static class FluentValidationResultExtensions
    {
        /// <summary>
        /// Валидирует входную модель и возвращает <see cref="Result{T,U}"/>:
        /// успех с самой моделью, либо ошибку <see cref="ErrorType.Validation"/>
        /// со списком <see cref="ValidationErrorDetail"/> по каждому
        /// нарушенному правилу.
        /// </summary>
        public static async Task<Result<T, Error>> ValidateToResultAsync<T>(
            this IValidator<T> validator,
            T instance,
            CancellationToken ct = default)
        {
            ValidationResult validationResult = await validator.ValidateAsync(instance, ct);
            if (validationResult.IsValid)
                return Success<T, Error>(instance);

            IReadOnlyList<ValidationErrorDetail> details = validationResult.Errors
                .Select(e => new ValidationErrorDetail(e.PropertyName, e.ErrorMessage))
                .ToList();

            return Failure<T>(Error.Validation("Входные данные не прошли валидацию.", details));
        }
    }
}
