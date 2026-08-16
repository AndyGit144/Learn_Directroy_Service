using Microsoft.AspNetCore.Http;
using System.Net;

namespace DirectoryService.WebApi.Common
{
    /// <summary>
    /// Маппинг Result -&gt; Envelope (задача 13, п.4). Конвертер в зависимости
    /// от статуса Result и типа ошибки формирует нужный Envelope и HTTP-статус
    /// код, чтобы контроллеру не нужно было писать try/catch или switch на
    /// каждый эндпоинт.
    /// </summary>
    public static class ResultToEnvelopeExtensions
    {
        /// <summary>
        /// Успешный Result с полезной нагрузкой -&gt; 200 OK с Envelope&lt;Y&gt;.
        /// Неуспешный -&gt; соответствующий типу ошибки статус код.
        /// </summary>
        /// <param name="result">Результат обработчика команды/запроса.</param>
        /// <param name="successMapper">Преобразование успешного значения в контракт ответа.</param>
        public static IResult ToApiResult<T, Y>(this Result<T, Error> result, Func<T, Y> successMapper)
        {
            if (result.IsSuccess)
                return Results.Ok(Envelope<Y>.Success(successMapper(result.OnSuccess)));

            return Results.Json(
                Envelope<Y>.Failure(result.OnError.Message),
                statusCode: (int)StatusCodeFromError(result.OnError));
        }

        /// <summary>
        /// Успешный Result без полезной нагрузки (<see cref="Nothing"/>) -&gt; 204 No Content.
        /// Неуспешный -&gt; соответствующий типу ошибки статус код.
        /// </summary>
        public static IResult ToApiResult(this Result<Nothing, Error> result)
        {
            if (result.IsSuccess)
                return Results.NoContent();

            return Results.Json(
                Envelope<object>.Failure(result.OnError.Message),
                statusCode: (int)StatusCodeFromError(result.OnError));
        }

        private static HttpStatusCode StatusCodeFromError(Error error) => error.Type switch
        {
            ErrorType.NotFound => HttpStatusCode.NotFound,
            ErrorType.Conflict => HttpStatusCode.Conflict,
            ErrorType.InternalError => HttpStatusCode.InternalServerError,
            ErrorType.Validation => HttpStatusCode.BadRequest,
            ErrorType.InvalidFormat => HttpStatusCode.BadRequest,
            ErrorType.None => throw new InvalidOperationException("None error type specified in operation result."),
            _ => HttpStatusCode.InternalServerError
        };
    }
}
