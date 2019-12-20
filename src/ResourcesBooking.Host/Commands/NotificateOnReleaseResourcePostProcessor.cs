using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using ResourcesBooking.Host.Options;
using ResourcesBooking.Host.Services;

namespace ResourcesBooking.Host.Commands
{
    public class NotificateOnReleaseResourcePostProcessor : IRequestPostProcessor<ReleaseResourceCommand, ReleaseResourceResult>
    {
        private readonly INotificationService _notificationService;
        private readonly NotificationOptions _options;

        public NotificateOnReleaseResourcePostProcessor(INotificationService notificator, NotificationOptions options)
        {
            _notificationService = notificator;
            _options = options;
        }

        public async Task Process(ReleaseResourceCommand command, ReleaseResourceResult response, CancellationToken cancellationToken)
        {
            var resourceDisplayName = $"[{response.ResourceName}]({_options.Hostname}/Resources/Details/{response.ResourceId})";

            if (response.WhoBooked != null)
            {
                await _notificationService.Notify(response.WhoBooked, 
                    $"You've booked {resourceDisplayName} just now", cancellationToken);
            }

            if (command.SystemAction && response.WhoReleased != null)
            {
                await _notificationService.Notify(response.WhoReleased, 
                    $"{resourceDisplayName} has been released just now", cancellationToken);
            }
        }
    }
}