using System.Threading;
using System.Threading.Tasks;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Services
{
    public interface INotificationService
    {
         Task Notify(User user, string payload, CancellationToken cancellationToken);
    }
}