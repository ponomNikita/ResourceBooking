using System;
using MediatR;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands
{
    public class BookResourceCommand : IRequest<BookResourceResult>, IRequireSaveChanges
    {
        public BookResourceCommand(Guid resourceId, User bookedBy, string bookingReason, long bookingDurationInMinutes)
        {
            ResourceId = resourceId;
            BookedBy = bookedBy;
            BookingReason = bookingReason;
            BookingDurationInMinutes = bookingDurationInMinutes;
        }

        public Guid ResourceId { get; }

        public User BookedBy { get; }

        public string BookingReason { get; }
        
        public long BookingDurationInMinutes { get; }
    }
}