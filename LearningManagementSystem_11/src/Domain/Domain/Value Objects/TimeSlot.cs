using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Value_Objects
{
    public record TimeSlot
    {
        public TimeSpan StartTime { get; }
        public TimeSpan EndTime { get; }
        public TimeSpan Duration => EndTime - StartTime;

        private TimeSlot(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }

        public static TimeSlot Create(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime)
                throw new ArgumentException("Время начала должно быть раньше времени окончания");

            var duration = endTime - startTime;

            if (duration.TotalMinutes != 40 && duration.TotalMinutes != 45)
                throw new ArgumentException("Продолжительность урока должна быть 40 или 45 минут");

            return new TimeSlot(startTime, endTime);
        }

        public static TimeSlot CreateFromLessonNumber(short lessonNumber, int durationMinutes = 45)
        {
            if (lessonNumber < 1 || lessonNumber > 8)
                throw new ArgumentException("Номер урока должен быть от 1 до 8", nameof(lessonNumber));

            if (durationMinutes != 40 && durationMinutes != 45)
                throw new ArgumentException("Продолжительность может быть только 40 или 45 минут");

            // Начало первого урока в 8:00
            var baseStartTime = new TimeSpan(8, 0, 0);
            var breakDuration = TimeSpan.FromMinutes(10); // Перемена 10 минут
            var lessonDuration = TimeSpan.FromMinutes(durationMinutes);

            // Рассчитываем время начала
            var startTime = baseStartTime
                + TimeSpan.FromMinutes((lessonNumber - 1) * (durationMinutes + 10));

            var endTime = startTime + lessonDuration;

            return new TimeSlot(startTime, endTime);
        }

        public bool OverlapsWith(TimeSlot other)
        {
            return StartTime < other.EndTime && other.StartTime < EndTime;
        }
    }
}
