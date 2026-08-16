using Domain.Entities;
using Domain.Enums;
using Domain.Value_Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Aggregates.Student.Database.Configurations
{
    /// <summary>
    /// Конфигурация таблицы для агрегата <see cref="Student"/>.
    /// </summary>
    public sealed class StudentEntityConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // указываем с какой таблицей связать класс Student
            builder.ToTable("students");

            // устанавливаем первичный ключ
            builder.HasKey(x => x.Id).HasName("pk_students");

            builder
                .Property(x => x.Id)
                .HasColumnName("id");

            // ФИО ученика - сложный объект из трех примитивных полей
            builder.ComplexProperty(
                x => x.Name,
                cpb =>
                {
                    cpb.Property(n => n.FirstName)
                        .HasColumnName("first_name")
                        .HasMaxLength(100)
                        .IsRequired();

                    cpb.Property(n => n.LastName)
                        .HasColumnName("last_name")
                        .HasMaxLength(100)
                        .IsRequired();

                    cpb.Property(n => n.MiddleName)
                        .HasColumnName("middle_name")
                        .HasMaxLength(100)
                        .IsRequired();
                });

            builder
                .Property(x => x.DateOfBirth)
                .HasColumnName("date_of_birth")
                .IsRequired();

            builder
                .Property(x => x.ClassId)
                .HasColumnName("class_id")
                .IsRequired();

            // Email - кастомный класс из одного примитивного поля Value
            builder
                .Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(Email.MAX_EMAIL_LENGTH)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => Email.Create(fromDb));

            builder.HasIndex(x => x.Email).IsUnique();

            // PhoneNumber - кастомный класс из одного примитивного поля Value
            builder
                .Property(x => x.ParentPhone)
                .HasColumnName("parent_phone")
                .HasMaxLength(PhoneNumber.MAX_PHONE_LENGTH)
                .IsRequired()
                .HasConversion(toDb => toDb.Value, fromDb => PhoneNumber.Create(fromDb));

            builder
                .Property(x => x.HasSpecialNeeds)
                .HasColumnName("has_special_needs")
                .IsRequired();

            builder
                .Property(x => x.EnrollmentDate)
                .HasColumnName("enrollment_date")
                .IsRequired();

            // StudentStatus - "умное перечисление", храним как int (Key) в базе данных
            builder
                .Property(x => x.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasConversion(toDb => toDb.Key, fromDb => StudentStatus.FromKey(fromDb));

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
        }
    }
}
