using Domain.Value_Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Entities.Database.Configurations
{
    /// <summary>
    /// Конфигурация таблицы для сущности <see cref="SchoolClass"/>.
    /// </summary>
    public sealed class SchoolClassEntityConfiguration : IEntityTypeConfiguration<SchoolClass>
    {
        public void Configure(EntityTypeBuilder<SchoolClass> builder)
        {
            builder.ToTable("school_classes");

            builder.HasKey(x => x.Id).HasName("pk_school_classes");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            // ClassName - кастомный класс, чье строковое представление ("10А")
            // однозначно преобразуется обратно через ClassName.Parse()
            builder
                .Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => ClassName.Parse(fromDb));

            builder
                .Property(x => x.Specialization)
                .HasColumnName("specialization")
                .HasMaxLength(150)
                .IsRequired();

            builder
                .Property(x => x.ClassTeacherId)
                .HasColumnName("class_teacher_id")
                .IsRequired();

            builder
                .Property(x => x.AcademicYear)
                .HasColumnName("academic_year")
                .HasMaxLength(9)
                .IsRequired();

            builder
                .Property(x => x.MaxStudents)
                .HasColumnName("max_students")
                .IsRequired();

            builder
                .Property(x => x.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            // EntityLifetime - сложный объект из двух примитивных полей
            builder.ComplexProperty(
                x => x.Lifetime,
                cpb =>
                {
                    cpb.Property(l => l.CreatedAt)
                        .HasColumnName("created_at")
                        .IsRequired();

                    cpb.Property(l => l.UpdatedAt)
                        .HasColumnName("updated_at")
                        .IsRequired();
                });

            // Примечание: коллекция StudentIds - приватный список Guid без отдельной
            // сущности-обёртки, конфигурация примитивной коллекции выходит за рамки
            // этой лабораторной работы.
        }
    }
}
