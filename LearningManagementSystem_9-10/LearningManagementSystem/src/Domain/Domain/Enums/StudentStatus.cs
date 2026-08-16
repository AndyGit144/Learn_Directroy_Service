namespace Domain.Enums
{
    /// <summary>
    /// "Умное перечисление" статуса ученика.
    /// Заменяет собой обычный enum, добавляя поведение, специфичное для каждого статуса.
    /// </summary>
    public abstract class StudentStatus : Enumeration<StudentStatus>
    {
        public static readonly StudentStatus Active = new StudentStatusActive();
        public static readonly StudentStatus OnLeave = new StudentStatusOnLeave();
        public static readonly StudentStatus Expelled = new StudentStatusExpelled();

        protected StudentStatus(int key, string name) : base(key, name) { }

        /// <summary>Можно ли перевести ученика в другой класс, находясь в этом статусе.</summary>
        public abstract bool CanBeTransferred();

        /// <summary>Может ли ученик уйти в академический отпуск, находясь в этом статусе.</summary>
        public abstract bool CanTakeLeave();

        /// <summary>Можно ли отчислить ученика, находясь в этом статусе.</summary>
        public abstract bool CanBeExpelled();
    }

    public sealed class StudentStatusActive : StudentStatus
    {
        public StudentStatusActive() : base(1, "Активен") { }

        public override bool CanBeTransferred() => true;
        public override bool CanTakeLeave() => true;
        public override bool CanBeExpelled() => true;
    }

    public sealed class StudentStatusOnLeave : StudentStatus
    {
        public StudentStatusOnLeave() : base(2, "В академическом отпуске") { }

        public override bool CanBeTransferred() => false;
        public override bool CanTakeLeave() => false;
        public override bool CanBeExpelled() => true;
    }

    public sealed class StudentStatusExpelled : StudentStatus
    {
        public StudentStatusExpelled() : base(3, "Отчислен") { }

        public override bool CanBeTransferred() => false;
        public override bool CanTakeLeave() => false;
        public override bool CanBeExpelled() => false;
    }
}
