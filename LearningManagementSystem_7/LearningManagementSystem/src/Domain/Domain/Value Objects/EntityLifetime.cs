using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Value_Objects
{
    public record EntityLifetime
    {
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; private set; }

        private EntityLifetime(DateTime createdAt, DateTime updatedAt)
        {
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public static EntityLifetime Create()
        {
            var now = DateTime.UtcNow;
            return new EntityLifetime(now, now);
        }

        public static EntityLifetime CreateFrom(DateTime createdAt, DateTime updatedAt)
        {
            if (createdAt > DateTime.UtcNow)
                throw new ArgumentException("Дата создания не может быть в будущем", nameof(createdAt));

            if (updatedAt > DateTime.UtcNow)
                throw new ArgumentException("Дата обновления не может быть в будущем", nameof(updatedAt));

            if (createdAt > updatedAt)
                throw new ArgumentException("Дата создания не может быть позже даты обновления");

            return new EntityLifetime(createdAt, updatedAt);
        }

        public EntityLifetime MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
            return this;
        }
    }
}
