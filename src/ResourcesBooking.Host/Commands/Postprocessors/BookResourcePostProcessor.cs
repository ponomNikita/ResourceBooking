using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands.Postprocessors
{
    public class BookResourcePostProcessor : IRequestPostProcessor<BookResourceCommand, Unit>
    {
        private readonly ResourcesContext _context;

        public BookResourcePostProcessor(ResourcesContext context)
        {
            _context = context;
        }

        public async Task Process(BookResourceCommand command, Unit response, CancellationToken cancellationToken)
        {
            var history = HistoryEntry.Create(Guid.NewGuid(), 
                $"Resource booked. Reason: \"{command.BookingReason}\"",
                command.BookedBy.Login,
                command.ResourceId,
                DateTimeOffset.UtcNow);

            await _context.AddAsync(history);
        }
    }
}