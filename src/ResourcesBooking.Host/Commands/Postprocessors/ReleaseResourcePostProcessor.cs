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
            var userLogin = command.SystemAction ? User.GetSystemUser().Login : response.WhoReleased.Login;

            var releasedHistory = HistoryEntry.Create(Guid.NewGuid(), 
                "Resource was released.",
                userLogin,
                command.ResourceId,
                DateTimeOffset.UtcNow);

            await _context.History.AddAsync(releasedHistory);

            if (response.WhoBooked != null)
            {
                var bookedHistory = HistoryEntry.Create(Guid.NewGuid(), 
                    $"Resource was booked. Reason: \"{response.BookingReason}\".",
                    response.WhoBooked.Login,
                    command.ResourceId,
                    DateTimeOffset.UtcNow);

                await _context.History.AddAsync(bookedHistory);
            }
        }
    }
}