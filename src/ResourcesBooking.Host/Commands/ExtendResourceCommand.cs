using System;
using MediatR;

namespace ResourcesBooking.Host.Commands
{
    public class ExtendResourceCommand : IRequest<ExtendResourceResult>
    {
        public ExtendResourceCommand(Guid resourceId, long extendTimeInMinutes)
        {
            ExtendTimeInMinutes = extendTimeInMinutes;
            ResourceId = resourceId;
        }

        public long ExtendTimeInMinutes { get; }
        public Guid ResourceId { get; }
    }

    public class ExtendResourceResult
    {

    }
}