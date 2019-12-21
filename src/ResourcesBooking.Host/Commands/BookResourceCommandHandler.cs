using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ResourcesBooking.Host.Commands
{
    public class BookResourceCommandHandler : IRequestHandler<BookResourceCommand, Unit>
    {
        private readonly ResourcesContext _context;

        public BookResourceCommandHandler(ResourcesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(BookResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == command.ResourceId);

            resource.Book(
                command.BookedBy, 
                command.BookingDurationInMinutes,
                command.BookingReason);
                
            Log.Information("{@user} booked resource {@resource}", command.BookedBy.Login, resource.Name);

            return Unit.Value;
        }
    }
}