namespace Domain.Enums
{
    /// <summary>
    /// "Умное перечисление" типа оценки (зачётной системы).
    /// Используется, например, для предметов/занятий, где оценивание идёт не по 5-балльной
    /// шкале (см. <see cref="Domain.Value_Objects.GradeValue"/>), а по системе "зачёт/незачёт".
    /// </summary>
    public abstract class GradeType : Enumeration<GradeType>
    {
        public static readonly GradeType Passed = new GradeTypePassed();
        public static readonly GradeType Failed = new GradeTypeFailed();

        protected GradeType(int key, string name) : base(key, name) { }

        /// <summary>Считается ли такой результат положительным (успешным прохождением).</summary>
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
