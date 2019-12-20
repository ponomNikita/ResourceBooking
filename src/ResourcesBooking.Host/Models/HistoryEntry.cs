using System;

namespace ResourcesBooking.Host.Models
{
    public class HistoryEntry
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public User User { get; set; }

        public Resource Resource { get; set; }

        public Guid ResourceId { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}