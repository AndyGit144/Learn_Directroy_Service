using DirectoryService.WebApi.Common;
using DirectoryService.WebApi.Models;
using DirectoryService.WebApi.Storage;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DirectoryService.WebApi.Controllers.Students
{
    /// <summary>
    /// Контроллер для работы с учениками (демонстрация CRUD на ASP.NET Core Web API).
    /// </summary>
    [ApiController]
    [Route("api/students")]
    public sealed class StudentsController : ControllerBase
    {
        /// <summary>
        /// Получает список всех учеников.
        /// </summary>
        /// <returns>Список учеников.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Получить всех учеников")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IResult GetStudents()
        {
            var students = Students.GetAll()
                .Select(ToResponse)
                .ToList();

            return Results.Ok(Envelope<IReadOnlyCollection<StudentResponse>>.Success(students));
        }

        /// <summary>
        /// Получает ученика по его уникальному идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор ученика.</param>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(Summary = "Получить ученика по идентификатору")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IResult GetStudent([FromRoute] Guid id)
        {
            var student = Students.GetById(id);
            if (student is null)
                return Results.NotFound(Envelope<StudentResponse>.Failure($"Не найден ученик с ID: {id}"));

            return Results.Ok(Envelope<StudentResponse>.Success(ToResponse(student)));
        }

        /// <summary>
        /// Создаёт нового ученика.
        /// </summary>
        /// <param name="request">Данные для создания ученика.</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Создать нового ученика")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IResult CreateStudent([FromBody] CreateStudentRequest request)
        {
            var validationError = Validate(
                request.FirstName, request.LastName, request.Age,
                request.ClassId, request.Email, request.ParentPhone);

            if (validationError is not null)
                return Results.BadRequest(Envelope<StudentResponse>.Failure(validationError));

            var student = new Student
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                ClassId = request.ClassId,
                Email = request.Email,
                ParentPhone = request.ParentPhone,
                HasSpecialNeeds = request.HasSpecialNeeds,
            };

            Students.Add(student);

            return Results.Created($"api/students/{student.Id}", Envelope<StudentResponse>.Success(ToResponse(student)));
        }

        /// <summary>
        /// Полностью обновляет данные ученика.
        /// </summary>
        /// <param name="request">Новые данные ученика (Id обязателен).</param>
        [HttpPut]
        [SwaggerOperation(Summary = "Полностью обновить данные ученика")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IResult UpdateStudent([FromBody] UpdateStudentRequest request)
        {
            var validationError = Validate(
                request.FirstName, request.LastName, request.Age,
                request.ClassId, request.Email, request.ParentPhone);

            if (validationError is not null)
                return Results.BadRequest(Envelope<StudentResponse>.Failure(validationError));

            var student = Students.GetById(request.Id);
            if (student is null)
                return Results.NotFound(Envelope<StudentResponse>.Failure($"Не найден ученик с ID: {request.Id}"));

            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.Age = request.Age;
            student.ClassId = request.ClassId;
            student.Email = request.Email;
            student.ParentPhone = request.ParentPhone;
            student.HasSpecialNeeds = request.HasSpecialNeeds;

            return Results.NoContent();
        }

        /// <summary>
        /// Переводит ученика в другой класс (частичное обновление).
        /// </summary>
        /// <param name="id">Идентификатор ученика (берётся из маршрута).</param>
        /// <param name="request">Идентификатор нового класса.</param>
        [HttpPatch("{id:guid}/class")]
        [SwaggerOperation(Summary = "Перевести ученика в другой класс")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IResult ChangeStudentClass([FromRoute] Guid id, [FromBody] ChangeStudentClassRequest request)
        {
            if (request.NewClassId == Guid.Empty)
                return Results.BadRequest(Envelope<StudentResponse>.Failure("Идентификатор нового класса не может быть пустым."));

            var student = Students.GetById(id);
            if (student is null)
                return Results.NotFound(Envelope<StudentResponse>.Failure($"Не найден ученик с ID: {id}"));

            student.ClassId = request.NewClassId;

            return Results.NoContent();
        }

        /// <summary>
        /// Удаляет ученика по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор ученика.</param>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(Summary = "Удалить ученика")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IResult DeleteStudent([FromRoute] Guid id)
        {
            var student = Students.GetById(id);
            if (student is null)
                return Results.NotFound(Envelope<StudentResponse>.Failure($"Не найден ученик с ID: {id}"));

            Students.Remove(id);

            return Results.NoContent();
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

        private static string? Validate(
            string firstName,
            string lastName,
            int age,
            Guid classId,
            string email,
            string parentPhone)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return "Имя ученика не может быть пустым.";

            if (string.IsNullOrWhiteSpace(lastName))
                return "Фамилия ученика не может быть пустой.";

            if (age is < 5 or > 25)
                return "Возраст ученика должен быть от 5 до 25 лет.";

            if (classId == Guid.Empty)
                return "Ученик должен быть закреплён за классом.";

            if (string.IsNullOrWhiteSpace(email))
                return "Электронная почта ученика не может быть пустой.";

            if (string.IsNullOrWhiteSpace(parentPhone))
                return "Телефон родителя не может быть пустым.";

            return null;
        }
    }
}
