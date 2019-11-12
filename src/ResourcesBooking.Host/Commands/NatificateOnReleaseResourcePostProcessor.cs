using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using ResourcesBooking.Host.Models;
using ResourcesBooking.Host.Options;
using ResourcesBooking.Host.Services;

namespace ResourcesBooking.Host.Commands
{
    public class NatificateOnReleaseResourcePostProcessor : IRequestPostProcessor<ReleaseResourceCommand, ReleaseResourceResult>
    {
        private readonly IEnumerable<INotificationService> _notificators;
        private readonly NotificationOptions _options;

        public NatificateOnReleaseResourcePostProcessor(IEnumerable<INotificationService> notificators, NotificationOptions options)
        {
            _notificators = notificators;
            _options = options;
        }

        public async Task Process(ReleaseResourceCommand command, ReleaseResourceResult response, CancellationToken cancellationToken)
        {
            if (response.WhoBooked != null)
            {
                await Notify(response.WhoBooked, 
                    $"You've booked {response.ResourceName} just now", cancellationToken);
            }

            if (command.SystemAction && response.WhoReleased != null)
            {
                await Notify(response.WhoReleased, 
                    $"[{response.ResourceName}]({_options.Hostname}/Resources/Details/{response.ResourceId})" + 
                    $" has been released just now", cancellationToken);
            }
        }

        private async Task Notify(User user, string payload, CancellationToken cancellationToken)
        {
            foreach (var notifier in _notificators)
            {
                await notifier.Notify(user, payload, cancellationToken);
            }
        }
    }
}