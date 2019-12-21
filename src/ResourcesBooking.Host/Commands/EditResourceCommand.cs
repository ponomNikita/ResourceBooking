using System;
using System.ComponentModel.DataAnnotations;
using MediatR;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands
{
    public class EditResourceCommand : IRequest, IRequireSaveChanges
    {
        public EditResourceCommand(Resource resource)
        {
            this.Id = resource.Id;
            this.Name = resource.Name;
            this.Description = resource.Description;
        }

        public EditResourceCommand() {}

        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}