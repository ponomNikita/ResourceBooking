using System;
using MediatR;

namespace ResourcesBooking.Host.Commands
{
    public class RemoveResourceCommand : IRequest
    {
        public RemoveResourceCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}