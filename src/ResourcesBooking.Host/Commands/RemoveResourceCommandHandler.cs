using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Models;
using Serilog;

namespace ResourcesBooking.Host.Commands
{
    public class RemoveResourceCommandHandler : IRequestHandler<RemoveResourceCommand, Unit>
    {
        private readonly ResourcesContext _context;

        public RemoveResourceCommandHandler(ResourcesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == command.Id);

            if (resource is ResourcesGroup group)
            {
                await _context.Entry(group).Collection(it => it.Resources).LoadAsync(cancellationToken);

                _context.Remove(group);
            }
            else
            {
                _context.Remove(resource);
            }
                
            await _context.SaveChangesAsync();

            Log.Information("Resource {@resource} was removed", resource.Name);
            
            return Unit.Value;
        }
    }
}