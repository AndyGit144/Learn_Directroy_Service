namespace Domain.Enums
{
    /// <summary>
    /// "Умное перечисление" квалификационной категории учителя.
    /// </summary>
    public abstract class QualificationCategory : Enumeration<QualificationCategory>
    {
        public static readonly QualificationCategory First = new QualificationCategoryFirst();
        public static readonly QualificationCategory Highest = new QualificationCategoryHighest();

        protected QualificationCategory(int key, string name) : base(key, name) { }

        /// <summary>Минимальный стаж работы (в годах), необходимый для присвоения категории.</summary>
        public abstract int RequiredExperienceYears { get; }

        /// <summary>Может ли учитель с такой категорией быть наставником (куратором) молодых специалистов.</summary>
        public abstract bool CanMentorYoungTeachers();
    }

    public sealed class QualificationCategoryFirst : QualificationCategory
    {
        public QualificationCategoryFirst() : base(1, "Первая") { }

        public override int RequiredExperienceYears => 2;
        public override bool CanMentorYoungTeachers() => false;
    }

    public sealed class QualificationCategoryHighest : QualificationCategory
    {
        public QualificationCategoryHighest() : base(2, "Высшая") { }

        public override int RequiredExperienceYears => 5;
        public override bool CanMentorYoungTeachers() => true;
    }
}
