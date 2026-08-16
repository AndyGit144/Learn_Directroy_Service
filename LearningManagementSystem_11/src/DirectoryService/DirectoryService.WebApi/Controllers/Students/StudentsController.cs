using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Students;
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
    /// Все обработчики и хранилище доставляются через Dependency Injection
    /// (см. задачу 9, а так же регистрацию зависимостей в Program.cs).
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
        public IResult GetStudents([FromServices] GetStudentsQueryHandler handler)
        {
            var students = handler.Handle(new GetStudentsQuery())
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
        public IResult GetStudent([FromRoute] Guid id, [FromServices] GetStudentByIdQueryHandler handler)
        {
            try
            {
                var student = handler.Handle(new GetStudentByIdQuery(id));
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
        public IResult CreateStudent(
            [FromBody] CreateStudentRequest request,
            [FromServices] CreateStudentCommandHandler handler)
        {
            var command = new CreateStudentCommand(
                request.FirstName,
                request.LastName,
                request.Age,
                request.ClassId,
                request.Email,
                request.ParentPhone,
                request.HasSpecialNeeds);

            try
            {
                var student = handler.Handle(command);
                return Results.Created($"api/students/{student.Id}", Envelope<StudentResponse>.Success(ToResponse(student)));
            }
            catch (UseCaseValidationException ex)
            {
                return Results.BadRequest(Envelope<StudentResponse>.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Полностью обновляет данные ученика.
        /// </summary>
        [HttpPut]
        [SwaggerOperation(Summary = "Полностью обновить данные ученика")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IResult UpdateStudent(
            [FromBody] UpdateStudentRequest request,
            [FromServices] UpdateStudentCommandHandler handler)
        {
            var command = new UpdateStudentCommand(
                request.Id,
                request.FirstName,
                request.LastName,
                request.Age,
                request.ClassId,
                request.Email,
                request.ParentPhone,
                request.HasSpecialNeeds);

            try
            {
                handler.Handle(command);
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
        /// </summary>
        [HttpPatch("{id:guid}/class")]
        [SwaggerOperation(Summary = "Перевести ученика в другой класс")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IResult ChangeStudentClass(
            [FromRoute] Guid id,
            [FromBody] ChangeStudentClassRequest request,
            [FromServices] ChangeStudentClassCommandHandler handler)
        {
            try
            {
                handler.Handle(new ChangeStudentClassCommand(id, request.NewClassId));
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
        public IResult DeleteStudent([FromRoute] Guid id, [FromServices] DeleteStudentCommandHandler handler)
        {
            try
            {
                handler.Handle(new DeleteStudentCommand(id));
                return Results.NoContent();
            }
            catch (UseCaseNotFoundException ex)
            {
                return Results.NotFound(Envelope<StudentResponse>.Failure(ex.Message));
            }
        }

        private static StudentResponse ToResponse(Student student) => new(
            student.Id,
            student.FirstName,
            student.LastName,
            student.Age,
            student.ClassId,
            student.Email,
            student.ParentPhone,
            student.HasSpecialNeeds);
    }
}
