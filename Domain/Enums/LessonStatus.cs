namespace Domain.Enums
{
    /// <summary>
    /// "Умное перечисление" статуса урока.
    /// Заменяет собой обычный enum, добавляя поведение, специфичное для каждого статуса.
    /// </summary>
    public abstract class LessonStatus : Enumeration<LessonStatus>
    {
        public static readonly LessonStatus Scheduled = new LessonStatusScheduled();
        public static readonly LessonStatus Completed = new LessonStatusCompleted();
        public static readonly LessonStatus Cancelled = new LessonStatusCancelled();
        public static readonly LessonStatus Replaced = new LessonStatusReplaced();

        protected LessonStatus(int key, string name) : base(key, name) { }

        /// <summary>Является ли статус финальным (урок больше нельзя изменить).</summary>
        public abstract bool IsFinal { get; }

        /// <summary>Можно ли назначить замену учителю, находясь в этом статусе.</summary>
        public abstract bool CanAssignReplacement();
    }

    public sealed class LessonStatusScheduled : LessonStatus
    {
        public LessonStatusScheduled() : base(1, "Запланирован") { }

        public override bool IsFinal => false;
        public override bool CanAssignReplacement() => true;
    }

    public sealed class LessonStatusCompleted : LessonStatus
    {
        public LessonStatusCompleted() : base(2, "Завершен") { }

        public override bool IsFinal => true;
        public override bool CanAssignReplacement() => false;
    }

    public sealed class LessonStatusCancelled : LessonStatus
    {
        public LessonStatusCancelled() : base(3, "Отменен") { }

        public override bool IsFinal => true;
        public override bool CanAssignReplacement() => false;
    }

    public sealed class LessonStatusReplaced : LessonStatus
    {
        public LessonStatusReplaced() : base(4, "Заменен") { }

        public override bool IsFinal => false;
        public override bool CanAssignReplacement() => true;
    }
}
