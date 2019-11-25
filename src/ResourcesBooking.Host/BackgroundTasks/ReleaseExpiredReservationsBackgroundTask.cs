using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Commands;
using Serilog;

namespace ResourcesBooking.Host.BackgroundTasks
{
    public class ReleaseExpiredReservationsBackgroundTask : IBackgroundTask
    {
        private readonly IMediator _mediator;
        private readonly ResourcesContext _context;

        public ReleaseExpiredReservationsBackgroundTask(IMediator mediator, ResourcesContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            Log.Information("Check for expired reservation");

            var now = DateTimeOffset.UtcNow;

            var resourcesToRelease = await _context.Resources
                .WithDetails()
                .Where(it => it.BookedUntil < now)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var resource in resourcesToRelease)
            {
                await _mediator.Send(new ReleaseResourceCommand(resource.Id, resource.BookedBy, true));
            }
        }
    }
}