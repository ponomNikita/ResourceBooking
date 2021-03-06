using System;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands
{
    public class ReleaseResourceResult
    {
        public ReleaseResourceResult(string resourceName, Guid resourceId, User whoReleased, User whoBooked, string bookingReason)
        {
            ResourceName = resourceName;
            ResourceId = resourceId;
            WhoReleased = whoReleased;
            WhoBooked = whoBooked;
            BookingReason = bookingReason;
        }

        public string ResourceName { get; }
        
        public Guid ResourceId { get; }

        public User WhoReleased { get; set; } 

        public User WhoBooked { get; set; }

        public string BookingReason { get; set; }
    }
}