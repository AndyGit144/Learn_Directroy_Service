using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Создать ученика". Содержит всю бизнес-логику операции:
    /// проверку бизнес-правил и взаимодействие с хранилищем. Контроллер про эту
    /// логику ничего не знает (см. задачу 10).
    /// </summary>
    public sealed class CreateStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;

        public CreateStudentCommandHandler(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public Student Handle(CreateStudentCommand command)
        {
            Validate(command);

            var student = new Student
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName,
                LastName = command.LastName,
                Age = command.Age,
                ClassId = command.ClassId,
                Email = command.Email,
                ParentPhone = command.ParentPhone,
                HasSpecialNeeds = command.HasSpecialNeeds,
            };

            _repository.Add(student);

            return student;
        }

        private static void Validate(CreateStudentCommand command)
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
