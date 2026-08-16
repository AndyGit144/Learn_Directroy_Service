using FluentValidation;

namespace DirectoryService.UseCases.Validation
{
    /// <summary>
    /// Адаптер между бизнес-правилами доменного слоя и FluentValidation
    /// (задача 14). Доменные Value Object'ы и агрегаты уже содержат всю
    /// нужную валидацию в своих фабричных методах <c>Create(...)</c> (длины
    /// строк, регулярные выражения, диапазоны и т.д.) — эти правила бросают
    /// <see cref="ArgumentException"/>. Чтобы не дублировать их отдельным
    /// набором FluentValidation-правил, этот extension-метод перехватывает
    /// исключение доменной фабрики и превращает его в обычный
    /// FluentValidation ValidationFailure, привязанный к конкретному свойству
    /// входной команды/DTO.
    /// </summary>
    public static class DomainRuleValidatorExtensions
    {
        /// <summary>
        /// Прогоняет значение свойства через доменную фабрику/бизнес-правило.
        /// Если фабрика бросает <see cref="ArgumentException"/> — правило
        /// FluentValidation считается не пройденным, а сообщение из
        /// исключения становится текстом ошибки валидации.
        /// </summary>
        /// <param name="domainFactory">
        /// Доменный фабричный метод (например, <c>Email.Create</c>) или иной
        /// метод, реализующий бизнес-правило и бросающий <see cref="ArgumentException"/>
        /// при его нарушении.
        /// </param>
        /// <param name="propertyName">
        /// Имя свойства, к которому нужно привязать ошибку. Если не указано,
        /// используется имя свойства, для которого вызван RuleFor(...)
        /// (для правил уровня всей команды, например RuleFor(x =&gt; x),
        /// имя стоит указывать явно).
        /// </param>
        public static IRuleBuilderOptions<T, TProperty> MustSatisfyDomainRule<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            Action<TProperty> domainFactory,
            string? propertyName = null)
        {
            return ruleBuilder.Custom((value, context) =>
            {
                try
                {
                    domainFactory(value);
                }
                catch (ArgumentException ex)
                {
                    context.AddFailure(propertyName ?? context.PropertyName, ex.Message);
                }
            });
        }
    }
}
