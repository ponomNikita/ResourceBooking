using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ResourcesBooking.Host.Commands
{
    public class ExtendResourceCommandHandler : IRequestHandler<ExtendResourceCommand, ExtendResourceResult>
    {
        private readonly ResourcesContext _context;

        public ExtendResourceCommandHandler(ResourcesContext context)
        {
            _context = context;
        }
        public async Task<ExtendResourceResult> Handle(ExtendResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == command.ResourceId);

            resource.Extend(command.ExtendTimeInMinutes);

            await _context.SaveChangesAsync();

            Log.Information("{@user} extended resource {@resource} for {@extendTime} minutes", 
                resource.BookedBy.Login, 
                command.ExtendTimeInMinutes, 
                resource.Name);

            return new ExtendResourceResult();
        }
    }
}