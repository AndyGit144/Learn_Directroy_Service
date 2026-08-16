using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    namespace LMS.Domain.Aggregates
    {
        using global::Domain.Value_Objects;
        using LMS.Domain.Enums;

        public class Lesson
        {
            public Guid Id { get; }
            public Guid SubjectId { get; }
            public Guid TeacherId { get; private set; }
            public Guid ClassId { get; }
            public Guid ClassRoomId { get; private set; }
            public DateTime Date { get; }
            public TimeSlot TimeSlot { get; }
            public short LessonNumber { get; }
            public string Topic { get; private set; }
            public LessonStatus Status { get; private set; }
            public Guid? ReplacementTeacherId { get; private set; }
            public EntityLifetime Lifetime { get; private set; }

            // Связанные сущности агрегата
            private List<Guid> _assignmentIds = new();
            private List<Guid> _gradeIds = new();

            public IReadOnlyCollection<Guid> AssignmentIds => _assignmentIds.AsReadOnly();
            public IReadOnlyCollection<Guid> GradeIds => _gradeIds.AsReadOnly();

            private Lesson(
                Guid id,
                Guid subjectId,
                Guid teacherId,
                Guid classId,
                Guid classRoomId,
                DateTime date,
                TimeSlot timeSlot,
                short lessonNumber,
                string topic)
            {
                Id = id;
                SubjectId = subjectId;
                TeacherId = teacherId;
                ClassId = classId;
                ClassRoomId = classRoomId;
                Date = date;
                TimeSlot = timeSlot;
                LessonNumber = lessonNumber;
                Topic = topic;
                Status = LessonStatus.Scheduled;
                Lifetime = EntityLifetime.Create();
            }

            public static Lesson Create(
                Guid subjectId,
                Guid teacherId,
                Guid classId,
                Guid classRoomId,
                DateTime date,
                short lessonNumber,
                string topic,
                int durationMinutes = 45)
            {
                ValidateIds(subjectId, teacherId, classId, classRoomId);

                if (date.Date < DateTime.Today)
                    throw new ArgumentException("Нельзя создать занятие на прошедшую дату");

                if (string.IsNullOrWhiteSpace(topic))
                    throw new ArgumentException("Тема урока не может быть пустой");

                var timeSlot = TimeSlot.CreateFromLessonNumber(lessonNumber, durationMinutes);

                return new Lesson(
                    Guid.NewGuid(),
                    subjectId,
                    teacherId,
                    classId,
                    classRoomId,
                    date,
                    timeSlot,
                    lessonNumber,
                    topic);
            }

            public void UpdateTopic(string newTopic)
            {
                if (Status == LessonStatus.Completed)
                    throw new InvalidOperationException("Нельзя изменить тему завершенного урока");

                if (string.IsNullOrWhiteSpace(newTopic))
                    throw new ArgumentException("Тема урока не может быть пустой");

                Topic = newTopic;
                Lifetime = Lifetime.MarkAsUpdated();
            }

            public void AssignReplacementTeacher(Guid replacementTeacherId)
            {
                if (replacementTeacherId == Guid.Empty)
                    throw new ArgumentException("Идентификатор учителя не может быть пустым");

                if (Status == LessonStatus.Completed)
                    throw new InvalidOperationException("Нельзя назначить замену для завершенного урока");

                ReplacementTeacherId = replacementTeacherId;
                Status = LessonStatus.Replaced;
                Lifetime = Lifetime.MarkAsUpdated();
            }

            public void Cancel(string reason)
            {
                if (Status == LessonStatus.Completed)
                    throw new InvalidOperationException("Нельзя отменить завершенный урок");

                if (string.IsNullOrWhiteSpace(reason))
                    throw new ArgumentException("Необходимо указать причину отмены");

                Status = LessonStatus.Cancelled;
                Lifetime = Lifetime.MarkAsUpdated();
            }

            public void Complete()
            {
                if (Status == LessonStatus.Cancelled)
                    throw new InvalidOperationException("Нельзя завершить отмененный урок");

                if (Date.Date > DateTime.Today)
                    throw new InvalidOperationException("Нельзя завершить урок, который еще не проведен");

                Status = LessonStatus.Completed;
                Lifetime = Lifetime.MarkAsUpdated();
            }

            public void AddAssignment(Guid assignmentId)
            {
                if (Status == LessonStatus.Cancelled)
                    throw new InvalidOperationException("Нельзя добавить задание к отмененному уроку");

                if (_assignmentIds.Contains(assignmentId))
                    throw new InvalidOperationException("Это задание уже добавлено к уроку");

                _assignmentIds.Add(assignmentId);
                Lifetime = Lifetime.MarkAsUpdated();
            }

            public void AddGrade(Guid gradeId)
            {
                if (Status != LessonStatus.Completed)
                    throw new InvalidOperationException("Оценки можно добавлять только к завершенным урокам");

                _gradeIds.Add(gradeId);
                Lifetime = Lifetime.MarkAsUpdated();
            }

            public void ChangeClassRoom(Guid newClassRoomId)
            {
                if (newClassRoomId == Guid.Empty)
                    throw new ArgumentException("Идентификатор кабинета не может быть пустым");

                if (Status == LessonStatus.Completed)
                    throw new InvalidOperationException("Нельзя изменить кабинет завершенного урока");

                // Проверка: нельзя изменить кабинет менее чем за сутки
                if ((Date - DateTime.Now).TotalHours < 24)
                    throw new InvalidOperationException("Изменение кабинета возможно не позднее чем за 1 день до занятия");

                ClassRoomId = newClassRoomId;
                Lifetime = Lifetime.MarkAsUpdated();
            }

            private static void ValidateIds(Guid subjectId, Guid teacherId, Guid classId, Guid classRoomId)
            {
                if (subjectId == Guid.Empty)
                    throw new ArgumentException("Идентификатор предмета не может быть пустым");

                if (teacherId == Guid.Empty)
                    throw new ArgumentException("Идентификатор учителя не может быть пустым");

                if (classId == Guid.Empty)
                    throw new ArgumentException("Идентификатор класса не может быть пустым");

                if (classRoomId == Guid.Empty)
                    throw new ArgumentException("Идентификатор кабинета не может быть пустым");
            }
        }

        // Enum для статуса занятия
        namespace LMS.Domain.Enums
        {
            public enum LessonStatus
            {
                Scheduled = 1,
                Completed = 2,
                Cancelled = 3,
                Replaced = 4
            }

        }
    }
}
