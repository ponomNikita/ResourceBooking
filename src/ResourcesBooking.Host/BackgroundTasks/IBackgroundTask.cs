using System.Threading;
using System.Threading.Tasks;

namespace ResourcesBooking.Host.BackgroundTasks
{
    public interface IBackgroundTask
    {
        Task Execute(CancellationToken cancellationToken);    
    }
}