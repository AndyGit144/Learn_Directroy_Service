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

        /// <summary>Периодичность прохождения аттестации для подтверждения категории, в годах.</summary>
        public abstract int RecertificationPeriodYears { get; }
    }

    public sealed class QualificationCategoryFirst : QualificationCategory
    {
        public QualificationCategoryFirst() : base(1, "Первая") { }

        public override int RecertificationPeriodYears => 5;
    }

    public sealed class QualificationCategoryHighest : QualificationCategory
    {
        public QualificationCategoryHighest() : base(2, "Высшая") { }

        public override int RecertificationPeriodYears => 5;
    }
}
