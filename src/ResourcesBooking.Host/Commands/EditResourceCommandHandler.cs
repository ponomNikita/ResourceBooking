using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ResourcesBooking.Host.Commands
{
    public class EditResourceCommandHandler : IRequestHandler<EditResourceCommand>
    {
        private readonly ResourcesContext _context;

        public EditResourceCommandHandler(ResourcesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(EditResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == command.Id);

            resource.Update(command.Name, command.Description);
                
            await _context.SaveChangesAsync();

            Log.Information("Resource {@resource} was updated", resource.Name);

            return Unit.Value;
        }
    }
}