using Domain.Entities;
using DirectoryService.UseCases.Common;
using DirectoryService.UseCases.Validation;
using FluentValidation;

namespace DirectoryService.UseCases.Students.Commands
{
    /// <summary>
    /// Обработчик команды "Обновить контактные данные ученика".
    /// Задача 14: команда проверяется <see cref="UpdateStudentCommandValidator"/>
    /// до похода в репозиторий.
    /// </summary>
    public sealed class UpdateStudentCommandHandler
    {
        private readonly IStudentsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateStudentCommand> _validator;

        public UpdateStudentCommandHandler(
            IStudentsRepository repository,
            IUnitOfWork unitOfWork,
            IValidator<UpdateStudentCommand> validator)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Result<Student, Error>> Handle(UpdateStudentCommand command, CancellationToken ct = default)
        {
            Result<UpdateStudentCommand, Error> validation = await _validator.ValidateToResultAsync(command, ct);
            if (validation.IsFailure)
                return Failure<Student>(validation.OnError);

            Student? student = await _repository.GetByIdAsync(command.Id, ct: ct);
            if (student is null)
                return Failure<Student>(Error.NotFound($"Не найден ученик с ID: {command.Id}"));

            try
            {
                Email email = Email.Create(command.Email);
                PhoneNumber phone = PhoneNumber.Create(command.ParentPhone);

                // Меняем состояние отслеживаемого агрегата — ChangeTracker сам
                // пометит его как Modified.
                student.UpdateContactInfo(email, phone);
                student.SetSpecialNeeds(command.HasSpecialNeeds);
            }
            catch (ArgumentException ex)
            {
                return Failure<Student>(Error.Validation(ex.Message));
            }

            Result<Nothing, Error> saving = await _unitOfWork.SaveChangesAsync(ct);
            if (saving.IsFailure)
                return Failure<Student>(saving.OnError);

            return Success<Student, Error>(student);
        }
    }
}
