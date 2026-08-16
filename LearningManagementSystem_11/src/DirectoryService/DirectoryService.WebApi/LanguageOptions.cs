namespace DirectoryService.WebApi
{
    /// <summary>
    /// Пример класса настроек, демонстрирующий маппинг конфигурации
    /// (секция "LanguageOptions" в appsettings.json) в объект через Bind()/Get&lt;T&gt;().
    /// </summary>
    public sealed class LanguageOptions
    {
        public string ApplicationLanguage { get; set; } = string.Empty;
        public string IanaTimeZone { get; set; } = string.Empty;
    }
}
