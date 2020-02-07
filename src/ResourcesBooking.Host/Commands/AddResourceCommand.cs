using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands
{
    public class AddResourceCommand  : IRequest, IRequireSaveChanges
    {
        public AddResourceCommand(Resource resource)
        {
            this.Id = resource.Id;
            this.Name = resource.Name;
            this.Description = resource.Description;
        }

        public AddResourceCommand() {}

        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public Guid? GroupId { get; set; }
    }
}
