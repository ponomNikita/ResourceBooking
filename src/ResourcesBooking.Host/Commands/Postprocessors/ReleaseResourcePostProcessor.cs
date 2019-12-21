using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands.Postprocessors
{
    public class ReleaseResourcePostProcessor : IRequestPostProcessor<ReleaseResourceCommand, ReleaseResourceResult>
    {
        private readonly ResourcesContext _context;

        public ReleaseResourcePostProcessor(ResourcesContext context)
        {
            _context = context;
        }

        public async Task Process(ReleaseResourceCommand command, ReleaseResourceResult response, CancellationToken cancellationToken)
        {
            var history = HistoryEntry.Create(Guid.NewGuid(), 
                "Resource released.",
                command.BookedBy.Login,
                command.ResourceId,
                DateTimeOffset.UtcNow);

            await _context.History.AddAsync(history);
        }
    }
}