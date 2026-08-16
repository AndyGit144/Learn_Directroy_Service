namespace DirectoryService.WebApi.Controllers.Students
{
    /// <summary>
    /// Ответ с данными об ученике.
    /// </summary>
    /// <param name="Id">Уникальный идентификатор ученика.</param>
    /// <param name="FirstName">Имя ученика.</param>
    /// <param name="LastName">Фамилия ученика.</param>
    /// <param name="MiddleName">Отчество ученика.</param>
    /// <param name="Age">Возраст ученика (вычисляется из даты рождения).</param>
    /// <param name="ClassId">Идентификатор класса.</param>
    /// <param name="Email">Электронная почта ученика.</param>
    /// <param name="ParentPhone">Телефон родителя/опекуна.</param>
    /// <param name="HasSpecialNeeds">Признак наличия особых образовательных потребностей.</param>
    /// <param name="Status">Текущий статус ученика (Активен/В академическом отпуске/Отчислен).</param>
    public sealed record StudentResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string MiddleName,
        int Age,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds,
        string Status);

    /// <summary>
    /// Запрос на создание нового ученика.
    /// </summary>
    /// <param name="FirstName">Имя ученика.</param>
    /// <param name="LastName">Фамилия ученика.</param>
    /// <param name="MiddleName">Отчество ученика.</param>
    /// <param name="DateOfBirth">Дата рождения ученика (5-25 лет на текущий момент).</param>
    /// <param name="ClassId">Идентификатор класса.</param>
    /// <param name="Email">Электронная почта ученика.</param>
    /// <param name="ParentPhone">Телефон родителя/опекуна.</param>
    /// <param name="HasSpecialNeeds">Признак наличия особых образовательных потребностей.</param>
    public sealed record CreateStudentRequest(
        string FirstName,
        string LastName,
        string MiddleName,
        DateTime DateOfBirth,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);

    /// <summary>
    /// Запрос на обновление контактных данных ученика.
    /// </summary>
    /// <param name="Id">Идентификатор ученика, которого нужно обновить.</param>
    /// <param name="Email">Электронная почта ученика.</param>
    /// <param name="ParentPhone">Телефон родителя/опекуна.</param>
    /// <param name="HasSpecialNeeds">Признак наличия особых образовательных потребностей.</param>
    public sealed record UpdateStudentRequest(
        Guid Id,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);

    /// <summary>
    /// Запрос на перевод ученика в другой класс (частичное обновление).
    /// </summary>
    /// <param name="NewClassId">Идентификатор нового класса.</param>
    /// <param name="NewGrade">Параллель нового класса (например, 5 для "5 класс").</param>
    public sealed record ChangeStudentClassRequest(Guid NewClassId, short NewGrade);
}
