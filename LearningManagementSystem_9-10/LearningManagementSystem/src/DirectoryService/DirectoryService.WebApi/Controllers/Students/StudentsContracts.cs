namespace DirectoryService.WebApi.Controllers.Students
{
    /// <summary>
    /// Ответ с данными об ученике.
    /// </summary>
    /// <param name="Id">Уникальный идентификатор ученика.</param>
    /// <param name="FirstName">Имя ученика.</param>
    /// <param name="LastName">Фамилия ученика.</param>
    /// <param name="Age">Возраст ученика.</param>
    /// <param name="ClassId">Идентификатор класса.</param>
    /// <param name="Email">Электронная почта ученика.</param>
    /// <param name="ParentPhone">Телефон родителя/опекуна.</param>
    /// <param name="HasSpecialNeeds">Признак наличия особых образовательных потребностей.</param>
    public sealed record StudentResponse(
        Guid Id,
        string FirstName,
        string LastName,
        int Age,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);

    /// <summary>
    /// Запрос на создание нового ученика.
    /// </summary>
    /// <param name="FirstName">Имя ученика.</param>
    /// <param name="LastName">Фамилия ученика.</param>
    /// <param name="Age">Возраст ученика (5-25).</param>
    /// <param name="ClassId">Идентификатор класса.</param>
    /// <param name="Email">Электронная почта ученика.</param>
    /// <param name="ParentPhone">Телефон родителя/опекуна.</param>
    /// <param name="HasSpecialNeeds">Признак наличия особых образовательных потребностей.</param>
    public sealed record CreateStudentRequest(
        string FirstName,
        string LastName,
        int Age,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);

    /// <summary>
    /// Запрос на полное обновление данных ученика.
    /// </summary>
    /// <param name="Id">Идентификатор ученика, которого нужно обновить.</param>
    /// <param name="FirstName">Имя ученика.</param>
    /// <param name="LastName">Фамилия ученика.</param>
    /// <param name="Age">Возраст ученика (5-25).</param>
    /// <param name="ClassId">Идентификатор класса.</param>
    /// <param name="Email">Электронная почта ученика.</param>
    /// <param name="ParentPhone">Телефон родителя/опекуна.</param>
    /// <param name="HasSpecialNeeds">Признак наличия особых образовательных потребностей.</param>
    public sealed record UpdateStudentRequest(
        Guid Id,
        string FirstName,
        string LastName,
        int Age,
        Guid ClassId,
        string Email,
        string ParentPhone,
        bool HasSpecialNeeds);

    /// <summary>
    /// Запрос на перевод ученика в другой класс (частичное обновление).
    /// </summary>
    /// <param name="Id">Идентификатор ученика (берётся из маршрута).</param>
    /// <param name="NewClassId">Идентификатор нового класса.</param>
    public sealed record ChangeStudentClassRequest(Guid Id, Guid NewClassId);
}
