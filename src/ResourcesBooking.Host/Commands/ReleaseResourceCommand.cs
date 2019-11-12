using System;
using MediatR;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands
{
    public class ReleaseResourceCommand : IRequest
    {
        public ReleaseResourceCommand(Guid resourceId, User bookedBy)
        {
            ResourceId = resourceId;
            BookedBy = bookedBy;
        }

        public Guid ResourceId { get; }

        public User BookedBy { get; }
    }
}