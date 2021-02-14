using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Commands;
using ResourcesBooking.Host.Models;
using ResourcesBooking.Host.Options;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class DetailsModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IMediator _mediator;

        public DetailsModel(DatabaseContext context, IMediator mediator, BookingOptions _options)
        {
            _context = context;
            _mediator = mediator;
            Options = _options;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {            
            Resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == id);

            if (Resource == null) 
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<PartialViewResult> OnGetHistoryPartialAsync(Guid id, int limit, int offset)
        {
            var history = await _context.History.Where(it => it.ResourceId == id)
                .OrderByDescending(it => it.Date)
                .Include(it => it.User)
                .Skip(offset)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();

            return Partial("HistoryPartial", new HistoryPartialModel 
            {
                History = history
            });
        }

        public async Task<IActionResult> OnPostReleaseAsync(Guid id)
        {
            var user = await _context.GetOrAdd(User);

            await _mediator.Send(new ReleaseResourceCommand(id, user, false));

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> OnPostExtendAsync(Guid id)
        {
            await _mediator.Send(new ExtendResourceCommand(id, Options.MinBookingPeriodInMinutes));

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public bool CanBook(Resource resource) 
        {
            var currentUserLogin = User.Identity.Name;

            return resource.BookedBy?.Login != currentUserLogin &&
                    !resource.Queue.Any(q => q.BookedBy.Login == currentUserLogin);
        }        

        public bool CanExtend(Resource resource) 
        {
            var currentUserLogin = User.Identity.Name;

            return resource.BookedBy?.Login == currentUserLogin;
        }

        public bool CanRelease(Resource resource) 
        {
            return !CanBook(resource);
        }

        public Resource Resource { get; private set; }

        public BookingOptions Options { get; private set; }

        public DateTimeOffset? GetGeneralBookedUntil()
        {
            var bookedUntil = Resource.BookedUntil;

            if (bookedUntil.HasValue && 
                Resource.Queue != null)
            {
                foreach (var item in Resource.Queue)
                {
                    bookedUntil = bookedUntil.Value.AddMinutes(item.DurationInMinutes);
                }
            }

            return bookedUntil;
        }
    }
}
