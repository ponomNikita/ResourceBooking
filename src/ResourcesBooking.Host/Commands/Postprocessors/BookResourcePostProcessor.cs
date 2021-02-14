using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands.Postprocessors
{
    public class BookResourcePostProcessor : IRequestPostProcessor<BookResourceCommand, BookResourceResult>
    {
        private readonly DatabaseContext _context;

        public BookResourcePostProcessor(DatabaseContext context)
        {
            _context = context;
        }

        public async Task Process(BookResourceCommand command, BookResourceResult response, CancellationToken cancellationToken)
        {
            var description = response.IsUserInLine 
                ? "User got in line."
                : $"Resource was booked. Reason: \"{command.BookingReason}\".";

            var history = HistoryEntry.Create(Guid.NewGuid(), 
                description,
                command.BookedBy.Login,
                command.ResourceId,
                DateTimeOffset.UtcNow);

            await _context.AddAsync(history);
        }
    }
}