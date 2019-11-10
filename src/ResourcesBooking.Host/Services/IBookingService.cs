using System.Threading.Tasks;

namespace ResourcesBooking.Host.Services
{
    public interface IBookingService
    {
         Task Book(BookingModel model);

         Task Release(ReleaseModel model);
    }
}