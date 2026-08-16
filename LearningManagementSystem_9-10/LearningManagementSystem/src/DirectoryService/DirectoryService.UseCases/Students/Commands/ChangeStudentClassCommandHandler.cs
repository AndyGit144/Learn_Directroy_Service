using DirectoryService.UseCases.Common;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Перевести ученика в другой класс".
    /// Если в будущем появится новое бизнес-правило (например, "нельзя переводить
    /// ученика с открытыми задолженностями"), его нужно будет добавить только здесь,
    /// не трогая контроллер (см. задачу 10).
    /// </summary>
    public sealed class ChangeStudentClassCommandHandler
    {
        private readonly IStudentsRepository _repository;

        public ChangeStudentClassCommandHandler(IStudentsRepository repository)
        {
            _repository = repository;
        }

        public Student Handle(ChangeStudentClassCommand command)
        {
            if (command.NewClassId == Guid.Empty)
                throw new UseCaseValidationException("Идентификатор нового класса не может быть пустым.");

            var student = _repository.GetById(command.StudentId);
            if (student is null)
                throw new UseCaseNotFoundException($"Не найден ученик с ID: {command.StudentId}");

            if (student.ClassId == command.NewClassId)
                throw new UseCaseValidationException("Ученик уже закреплён за этим классом.");

            student.ClassId = command.NewClassId;

            return student;
        }
    }
}
