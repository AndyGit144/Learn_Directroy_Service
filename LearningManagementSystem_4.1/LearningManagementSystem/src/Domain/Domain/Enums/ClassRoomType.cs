namespace Domain.Enums
{
    /// <summary>
    /// "Умное перечисление" типа учебного кабинета.
    /// </summary>
    public abstract class ClassRoomType : Enumeration<ClassRoomType>
    {
        public static readonly ClassRoomType ComputerLab = new ClassRoomTypeComputerLab();
        public static readonly ClassRoomType Laboratory = new ClassRoomTypeLaboratory();
        public static readonly ClassRoomType RegularClassroom = new ClassRoomTypeRegularClassroom();

        protected ClassRoomType(int key, string name) : base(key, name) { }

        /// <summary>Требует ли кабинет данного типа специального оборудования (техники, лабораторных стендов и т.п.).</summary>
        public abstract bool RequiresSpecialEquipment { get; }
    }

    public sealed class ClassRoomTypeComputerLab : ClassRoomType
    {
        public ClassRoomTypeComputerLab() : base(1, "Компьютерный класс") { }

        public override bool RequiresSpecialEquipment => true;
    }

    public sealed class ClassRoomTypeLaboratory : ClassRoomType
    {
        public ClassRoomTypeLaboratory() : base(2, "Лаборатория") { }

        public override bool RequiresSpecialEquipment => true;
    }

    public sealed class ClassRoomTypeRegularClassroom : ClassRoomType
    {
        public ClassRoomTypeRegularClassroom() : base(3, "Учебная аудитория") { }

        public override bool RequiresSpecialEquipment => false;
    }
}
