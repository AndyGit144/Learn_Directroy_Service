using Domain.Entities;
using DirectoryService.UseCases.Students.Commands;
using DirectoryService.UseCases.Students.Queries;
using DirectoryService.WebApi.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DirectoryService.WebApi.Controllers.Students
{
    /// <summary>
    /// Контроллер для работы с учениками.
    /// Контроллер не содержит бизнес-логики: он только принимает HTTP-запрос,
    /// передаёт данные в соответствующий обработчик команды/запроса (Use Case
    /// слой, CQRS) и мапит Result в Envelope (задача 13) — никаких try/catch
    /// для ожидаемых бизнес-ошибок, они уже приходят как Result.
    /// </summary>
    [ApiController]
    [Route("api/students")]
    public sealed class StudentsController : ControllerBase
    {
        /// <summary>
        /// Получает список всех учеников.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить всех учеников")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IResult> GetStudents(
            [FromServices] GetStudentsQueryHandler handler,
            CancellationToken ct)
        {
            var students = (await handler.Handle(new GetStudentsQuery(), ct))
                .Select(ToResponse)
                .ToList();

            return Results.Ok(Envelope<IReadOnlyCollection<StudentResponse>>.Success(students));
        }

        /// <summary>
        /// Получает ученика по его уникальному идентификатору.
        /// </summary>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Получить ученика по идентификатору")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> GetStudent(
            [FromRoute] Guid id,
            [FromServices] GetStudentByIdQueryHandler handler,
            CancellationToken ct)
        {
            Result<Student, Error> result = await handler.Handle(new GetStudentByIdQuery(id), ct);
            return result.ToApiResult(ToResponse);
        }

        /// <summary>
        /// Создаёт нового ученика.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать нового ученика")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IResult> CreateStudent(
            [FromBody] CreateStudentRequest request,
            [FromServices] CreateStudentCommandHandler handler,
            CancellationToken ct)
        {
            var command = new CreateStudentCommand(
                request.FirstName,
                request.LastName,
                request.MiddleName,
                request.DateOfBirth,
                request.ClassId,
                request.Email,
                request.ParentPhone,
                request.HasSpecialNeeds);

            Result<Student, Error> result = await handler.Handle(command, ct);
            return result.ToApiResult(ToResponse);
        }

        /// <summary>
        /// Обновляет контактные данные ученика.
        /// </summary>
        [HttpPut]
        [SwaggerOperation(Summary = "Обновить контактные данные ученика")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> UpdateStudent(
            [FromBody] UpdateStudentRequest request,
            [FromServices] UpdateStudentCommandHandler handler,
            CancellationToken ct)
        {
            var command = new UpdateStudentCommand(
                request.Id,
                request.Email,
                request.ParentPhone,
                request.HasSpecialNeeds);

            Result<Student, Error> result = await handler.Handle(command, ct);
            return result.ToApiResult(ToResponse);
        }

        /// <summary>
        /// Переводит ученика в другой класс (частичное обновление).
        /// Операция выполняется в транзакции с пессимистичной блокировкой
        /// строки ученика.
        /// </summary>
        [HttpPatch("{id:guid}/class")]
        [SwaggerOperation(Summary = "Перевести ученика в другой класс")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IResult> ChangeStudentClass(
            [FromRoute] Guid id,
            [FromBody] ChangeStudentClassRequest request,
            [FromServices] ChangeStudentClassCommandHandler handler,
            CancellationToken ct)
        {
            Result<Student, Error> result = await handler.Handle(
                new ChangeStudentClassCommand(id, request.NewClassId, request.NewGrade), ct);

            return result.ToApiResult(ToResponse);
        }

        /// <summary>
        /// Удаляет ученика по идентификатору.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Удалить ученика")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> DeleteStudent(
            [FromRoute] Guid id,
            [FromServices] DeleteStudentCommandHandler handler,
            CancellationToken ct)
        {
            Result<Nothing, Error> result = await handler.Handle(new DeleteStudentCommand(id), ct);
            return result.ToApiResult();
        }

        private static StudentResponse ToResponse(Student student) => new(
            student.Id,
            student.Name.FirstName,
            student.Name.LastName,
            student.Name.MiddleName,
            student.GetAge(),
            student.ClassId,
            student.Email.Value,
            student.ParentPhone.Value,
            student.HasSpecialNeeds,
            student.Status.Name);
    }
}
