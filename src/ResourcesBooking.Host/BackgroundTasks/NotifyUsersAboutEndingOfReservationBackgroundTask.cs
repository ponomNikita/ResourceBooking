using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Options;
using ResourcesBooking.Host.Services;
using Serilog;

namespace ResourcesBooking.Host.BackgroundTasks
{
    public class NotifyUsersAboutEndingOfReservationBackgroundTask : IBackgroundTask
    {
        private const int PartOfBackgroundTaskInterval = 55;
        private readonly DatabaseContext _context;
        private readonly INotificationService _notificator;
        private readonly NotificationOptions _options;

        public NotifyUsersAboutEndingOfReservationBackgroundTask(DatabaseContext context, 
            INotificationService notificator, 
            NotificationOptions options)
        {
            _context = context;
            _notificator = notificator;
            _options = options;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            Log.Information("Notifying about expired reservation");

            var minEndOfReservationPeriod = DateTimeOffset.Now
                .AddMinutes(_options.NotifyBeforeEndingOfReservationInMinutes);
            
            var maxEndOfReservationPeriod = minEndOfReservationPeriod.AddSeconds(PartOfBackgroundTaskInterval);

            var resourcesWithEndingOfReservation = await _context.Resources
                .WithDetails()
                .Where(it => it.BookedUntil < maxEndOfReservationPeriod &&
                       it.BookedUntil > minEndOfReservationPeriod)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            foreach (var resource in resourcesWithEndingOfReservation)
            {
                var resourceDisplayName = $"[{resource.Name}]({_options.Hostname}/Resources/Details/{resource.Id})";
                await _notificator.Notify(resource.BookedBy, 
                    $"Reservation of {resourceDisplayName} will end in {_options.NotifyBeforeEndingOfReservationInMinutes} minutes", 
                    cancellationToken);   
            }
        }
    }
}