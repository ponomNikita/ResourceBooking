using System;

namespace ResourcesBooking.Host.Models
{
    public class HistoryEntry
    {
        public static HistoryEntry Create(Guid id, string description, User user, Resource resource, DateTimeOffset date)
        {
            var history = Create(id, 
                description, 
                user?.Login ?? throw new ArgumentNullException(nameof(user)), 
                resource?.Id ?? throw new ArgumentNullException(nameof(resource)), 
                date);

            history.Resource = resource;

            return history;
        }

        public static HistoryEntry Create(Guid id, string description, string userLogin, Guid resourceId, DateTimeOffset date)
        {
            return new HistoryEntry
            {
                Id = id,
                Description = description,
                UserLogin = userLogin,
                ResourceId = resourceId,
                Date = date
            };
        }

        public Guid Id { get; set; }

        public string Description { get; set; }

        public User User { get; set; }

        public string UserLogin { get; set; }

        public Resource Resource { get; set; }

        public Guid ResourceId { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}