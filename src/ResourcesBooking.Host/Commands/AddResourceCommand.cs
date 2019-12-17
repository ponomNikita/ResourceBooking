using System;

namespace ResourcesBooking.Host.Commands
{
    public class AddResourceCommand : EditResourceCommand
    {
        public Guid? GroupId { get; set; }
    }
}