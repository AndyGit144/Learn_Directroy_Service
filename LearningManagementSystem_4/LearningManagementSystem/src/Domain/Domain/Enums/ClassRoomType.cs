namespace Domain.Enums
{
    /// <summary>
    /// "Умное перечисление" типа учебного кабинета.
    /// </summary>
    public abstract class ClassRoomType : Enumeration<ClassRoomType>
    {
        public static readonly ClassRoomType ComputerRoom = new ClassRoomTypeComputerRoom();
        public static readonly ClassRoomType Laboratory = new ClassRoomTypeLaboratory();
        public static readonly ClassRoomType RegularRoom = new ClassRoomTypeRegularRoom();

        protected ClassRoomType(int key, string name) : base(key, name) { }

        /// <summary>Требуется ли специализированное оборудование для проведения занятия в этом кабинете.</summary>
        public abstract bool RequiresSpecialEquipment { get; }
    }

    public sealed class ClassRoomTypeComputerRoom : ClassRoomType
    {
        public ClassRoomTypeComputerRoom() : base(1, "Компьютерный класс") { }

        public override bool RequiresSpecialEquipment => true;
    }

    public sealed class ClassRoomTypeLaboratory : ClassRoomType
    {
        public ClassRoomTypeLaboratory() : base(2, "Лаборатория") { }

        public override bool RequiresSpecialEquipment => true;
    }

    public sealed class ClassRoomTypeRegularRoom : ClassRoomType
    {
        public ClassRoomTypeRegularRoom() : base(3, "Учебная аудитория") { }

        public override bool RequiresSpecialEquipment => false;
    }
}
