using System.Net;
using DirectoryService.WebApi.Common;

namespace DirectoryService.WebApi.Middlewares
{
    /// <summary>
    /// Задача 13, п.5: глобальный middleware, перехватывающий необработанные
    /// исключения (действительно аварийные, непредвиденные ситуации — в
    /// отличие от ожидаемых бизнес-ошибок, которые обрабатываются Result
    /// паттерном и никогда сюда не долетают). Скрывает чувствительные детали
    /// исключения от клиента и возвращает единообразный Envelope с 500 статусом.
    /// </summary>
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Необработанное исключение при обработке запроса {Path}", httpContext.Request.Path);

                HttpStatusCode code = HttpStatusCode.InternalServerError;
                Envelope<object> envelope = Envelope<object>.Failure("Ошибка на стороне сервера.");

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = (int)code;
                await httpContext.Response.WriteAsJsonAsync(envelope);
            }
        }
    }

    /// <summary>
    /// Расширение для регистрации <see cref="ExceptionMiddleware"/> в конвейере (пайплайне).
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app) =>
            app.UseMiddleware<ExceptionMiddleware>();
    }
}
