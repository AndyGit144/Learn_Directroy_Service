using Domain.Entities.LMS.Domain.Aggregates;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Aggregates.Lesson.Database.Configurations
{
    /// <summary>
    /// Конфигурация таблицы для агрегата <see cref="Lesson"/>.
    /// </summary>
    public sealed class LessonEntityConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.ToTable("lessons");

            builder.HasKey(x => x.Id).HasName("pk_lessons");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            builder
                .Property(x => x.SubjectId)
                .HasColumnName("subject_id")
                .IsRequired();

            builder
                .Property(x => x.TeacherId)
                .HasColumnName("teacher_id")
                .IsRequired();

            builder
                .Property(x => x.ClassId)
                .HasColumnName("class_id")
                .IsRequired();

            builder
                .Property(x => x.ClassRoomId)
                .HasColumnName("class_room_id")
                .IsRequired();

            builder
                .Property(x => x.Date)
                .HasColumnName("date")
                .IsRequired();

            // TimeSlot - сложный объект из двух примитивных полей (время начала/окончания)
            builder.ComplexProperty(
                x => x.TimeSlot,
                cpb =>
                {
                    cpb.Property(t => t.StartTime)
                        .HasColumnName("start_time")
                        .IsRequired();

                    cpb.Property(t => t.EndTime)
                        .HasColumnName("end_time")
                        .IsRequired();
                });

            builder
                .Property(x => x.LessonNumber)
                .HasColumnName("lesson_number")
                .IsRequired();

            builder
                .Property(x => x.Topic)
                .HasColumnName("topic")
                .HasMaxLength(200)
                .IsRequired();

            // LessonStatus - "умное перечисление", храним как int (Key) в базе данных
            builder
                .Property(x => x.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasConversion(toDb => toDb.Key, fromDb => LessonStatus.FromKey(fromDb));

            builder
                .Property(x => x.ReplacementTeacherId)
                .HasColumnName("replacement_teacher_id")
                .IsRequired(false);

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

            // Примечание: коллекции AssignmentIds/GradeIds хранятся как приватные
            // списки Guid без отдельных сущностей-обёрток, поэтому конфигурация
            // их как примитивных коллекций (Assignment/Grade id-шники) выходит за
            // рамки этой лабораторной и будет добавлена вместе с сущностями
            // Assignment и Grade.
        }
    }
}
