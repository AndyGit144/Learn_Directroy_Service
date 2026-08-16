namespace Domain.Enums
{
    /// <summary>
    /// "Умное перечисление" типа зачётной оценки (зачёт/незачёт).
    /// Используется там, где вместо балльной оценки (см. GradeValue) нужен бинарный результат.
    /// </summary>
    public abstract class GradeType : Enumeration<GradeType>
    {
        public static readonly GradeType Passed = new GradeTypePassed();
        public static readonly GradeType Failed = new GradeTypeFailed();

        protected GradeType(int key, string name) : base(key, name) { }

        /// <summary>Считается ли оценка положительной (влияет на итоговую успеваемость).</summary>
        public abstract bool IsPassing { get; }
    }

    public sealed class GradeTypePassed : GradeType
    {
        public GradeTypePassed() : base(1, "Зачтено") { }

        public override bool IsPassing => true;
    }

    public sealed class GradeTypeFailed : GradeType
    {
        public GradeTypeFailed() : base(2, "Незачтено") { }

        public override bool IsPassing => false;
    }
}
