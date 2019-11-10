using System;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Services
{
    public class ReleaseModel
    {
        public ReleaseModel(Guid resourceId, User bookedBy)
        {
            ResourceId = resourceId;
            BookedBy = bookedBy;
        }

        public Guid ResourceId { get; }

        public User BookedBy { get; }

    }
}