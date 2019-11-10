using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ResourcesBooking.Host.Services
{
    public class BookingService : IBookingService
    {
        private readonly ResourcesContext _context;

        public BookingService(ResourcesContext context)
        {
            _context = context;
        }

        public async Task Book(BookingModel model)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == model.ResourceId);

            resource.Book(
                model.BookedBy, 
                model.BookingDurationInMinutes,
                model.BookingReason);
                
            await _context.SaveChangesAsync();

            Log.Information("{@user} booked resource {@resource}", model.BookedBy.Login, resource.Name);
        }

        public async Task Release(ReleaseModel model)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == model.ResourceId);

            resource.Release(model.BookedBy);

            await _context.SaveChangesAsync();

            Log.Information("{@user} released resource {@resource}", model.BookedBy.Login, resource.Name);
        }
    }
}