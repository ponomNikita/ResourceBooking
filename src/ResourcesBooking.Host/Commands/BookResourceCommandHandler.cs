using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ResourcesBooking.Host.Commands
{
    public class BookResourceCommandHandler : IRequestHandler<BookResourceCommand, BookResourceResult>
    {
        private readonly DatabaseContext _context;

        public BookResourceCommandHandler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<BookResourceResult> Handle(BookResourceCommand command, CancellationToken cancellationToken)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == command.ResourceId);

            resource.Book(
                command.BookedBy, 
                command.BookingDurationInMinutes,
                command.BookingReason);
                
            Log.Information("{@user} booked resource {@resource}", command.BookedBy.Login, resource.Name);

            var isUserInLine = !resource.BookedBy.Login.Equals(command.BookedBy.Login);

            return new BookResourceResult
            {
                IsUserInLine = isUserInLine
            };
        }
    }
}