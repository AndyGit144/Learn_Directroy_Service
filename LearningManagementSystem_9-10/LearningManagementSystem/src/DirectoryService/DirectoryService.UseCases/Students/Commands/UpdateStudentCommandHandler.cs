using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Полностью обновить данные ученика".
    /// </summary>
    public sealed class UpdateStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;

        public UpdateStudentCommandHandler(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public Student Handle(UpdateStudentCommand command)
        {
            Validate(command);

            var student = _repository.GetById(command.Id);
            if (student is null)
                throw new UseCaseNotFoundException($"Не найден ученик с ID: {command.Id}");

            student.FirstName = command.FirstName;
            student.LastName = command.LastName;
            student.Age = command.Age;
            student.ClassId = command.ClassId;
            student.Email = command.Email;
            student.ParentPhone = command.ParentPhone;
            student.HasSpecialNeeds = command.HasSpecialNeeds;

            return student;
        }

        private static void Validate(UpdateStudentCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.FirstName))
                throw new UseCaseValidationException("Имя ученика не может быть пустым.");

            if (string.IsNullOrWhiteSpace(command.LastName))
                throw new UseCaseValidationException("Фамилия ученика не может быть пустой.");

            if (command.Age is < 5 or > 25)
                throw new UseCaseValidationException("Возраст ученика должен быть от 5 до 25 лет.");

            if (command.ClassId == Guid.Empty)
                throw new UseCaseValidationException("Ученик должен быть закреплён за классом.");

            if (string.IsNullOrWhiteSpace(command.Email))
                throw new UseCaseValidationException("Электронная почта ученика не может быть пустой.");

            if (string.IsNullOrWhiteSpace(command.ParentPhone))
                throw new UseCaseValidationException("Телефон родителя не может быть пустым.");
        }
    }
}
