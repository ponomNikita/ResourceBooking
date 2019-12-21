using System;
using MediatR;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands
{
    public class ReleaseResourceCommand : IRequest<ReleaseResourceResult>, IRequireSaveChanges
    {
        public ReleaseResourceCommand(Guid resourceId, User bookedBy, bool systemAction)
        {
            ResourceId = resourceId;
            BookedBy = bookedBy;
            SystemAction = systemAction;

        }
        public Guid ResourceId { get; }

        public User BookedBy { get; }

        public bool SystemAction { get; }
    }
}