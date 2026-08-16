using Domain.Entities;
using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Students.Commands;
using DirectoryService.UseCases.Students.Queries;
using DirectoryService.WebApi.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DirectoryService.WebApi.Controllers.Students
{
    /// <summary>
    /// Контроллер для работы с учениками.
    /// Согласно задаче 10, контроллер не содержит бизнес-логики: он только
    /// принимает HTTP-запрос, передаёт данные в соответствующий обработчик
    /// команды/запроса (Use Case слой, CQRS) и возвращает результат.
    /// Начиная с задачи 12 все обработчики асинхронные — они работают с
    /// PostgreSQL через Entity Framework Core (Repository + Unit Of Work).
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
            try
            {
                var student = await handler.Handle(new GetStudentByIdQuery(id), ct);
                return Results.Ok(Envelope<StudentResponse>.Success(ToResponse(student)));
            }
            catch (UseCaseNotFoundException ex)
            {
                return Results.NotFound(Envelope<StudentResponse>.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Создаёт нового ученика.
        /// </summary>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать нового ученика")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

            try
            {
                var student = await handler.Handle(command, ct);
                return Results.Created($"api/students/{student.Id}", Envelope<StudentResponse>.Success(ToResponse(student)));
            }
            catch (UseCaseValidationException ex)
            {
                return Results.BadRequest(Envelope<StudentResponse>.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Обновляет контактные данные ученика.
        /// </summary>
        [HttpPut]
        [SwaggerOperation(Summary = "Обновить контактные данные ученика")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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

            try
            {
                await handler.Handle(command, ct);
                return Results.NoContent();
            }
            catch (UseCaseValidationException ex)
            {
                return Results.BadRequest(Envelope<StudentResponse>.Failure(ex.Message));
            }
            catch (UseCaseNotFoundException ex)
            {
                return Results.NotFound(Envelope<StudentResponse>.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Переводит ученика в другой класс (частичное обновление).
        /// Операция выполняется в транзакции с пессимистичной блокировкой
        /// строки ученика (задача 12).
        /// </summary>
        [HttpPatch("{id:guid}/class")]
        [SwaggerOperation(Summary = "Перевести ученика в другой класс")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IResult> ChangeStudentClass(
            [FromRoute] Guid id,
            [FromBody] ChangeStudentClassRequest request,
            [FromServices] ChangeStudentClassCommandHandler handler,
            CancellationToken ct)
        {
            try
            {
                await handler.Handle(new ChangeStudentClassCommand(id, request.NewClassId, request.NewGrade), ct);
                return Results.NoContent();
            }
            catch (UseCaseValidationException ex)
            {
                return Results.BadRequest(Envelope<StudentResponse>.Failure(ex.Message));
            }
            catch (UseCaseNotFoundException ex)
            {
                return Results.NotFound(Envelope<StudentResponse>.Failure(ex.Message));
            }
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
            try
            {
                await handler.Handle(new DeleteStudentCommand(id), ct);
                return Results.NoContent();
            }
            catch (UseCaseNotFoundException ex)
            {
                return Results.NotFound(Envelope<StudentResponse>.Failure(ex.Message));
            }
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
