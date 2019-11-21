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
        private readonly ResourcesContext _context;
        private readonly IMediator _mediator;

        public DetailsModel(ResourcesContext context, IMediator mediator, BookingOptions _options)
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
