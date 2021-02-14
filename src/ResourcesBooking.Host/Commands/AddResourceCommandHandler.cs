using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Models;
using Serilog;

namespace ResourcesBooking.Host.Commands
{
    public class AddResourceCommandHandler : IRequestHandler<AddResourceCommand, Unit>
    {
        private readonly DatabaseContext _context;

        public AddResourceCommandHandler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddResourceCommand command, CancellationToken cancellationToken)
        {
            if (command.GroupId.HasValue)
            {
                var groupId = command.GroupId.Value;
                var group = await _context.Groups.WithDetails().FirstOrDefaultAsync(it => it.Id == groupId);
                
                if (group == null)
                {
                    throw new Exception($"Group with id {groupId} not found");
                }

                var resource = new Resource
                {
                    Id = Guid.NewGuid(),
                    Name = command.Name,
                    Description = command.Description
                };

                group.Resources.Add(resource);
                await _context.Resources.AddAsync(resource, cancellationToken);
            }
            else
            {
                await _context.Groups.AddAsync(new ResourcesGroup
                {
                    Id = Guid.NewGuid(),
                    Name = command.Name,
                    Description = command.Description
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            Log.Information("Added resource: {@resource}", command);

            return Unit.Value;
        }
    }
}