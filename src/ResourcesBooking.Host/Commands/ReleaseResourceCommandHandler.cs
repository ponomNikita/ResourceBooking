using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ResourcesBooking.Host.Commands
{
    public class ReleaseResourceCommandHandler : IRequestHandler<ReleaseResourceCommand, ReleaseResourceResult>
    {
        private readonly DatabaseContext _context;

        public ReleaseResourceCommandHandler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<ReleaseResourceResult> Handle(ReleaseResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == command.ResourceId);

            resource.Release(command.BookedBy);

            Log.Information("{@user} released resource {@resource}", command.BookedBy.Login, resource.Name);

            return new ReleaseResourceResult(resource.Name, resource.Id, command.BookedBy, resource.BookedBy, resource.BookingReason);
        }
    }
}