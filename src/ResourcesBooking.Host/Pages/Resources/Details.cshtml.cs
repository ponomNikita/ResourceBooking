using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Models;
using ResourcesBooking.Host.Services;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class DetailsModel : PageModel
    {
        private readonly ResourcesContext _context;
        private readonly IBookingService _service;

        public DetailsModel(ResourcesContext context, IBookingService service)
        {
            _context = context;
            _service = service;
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

            await _service.Release(new ReleaseModel(id, user));

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public bool CanBook(Resource resource) 
        {
            var currentUserLogin = User.Identity.Name;

            return resource.BookedBy?.Login != currentUserLogin &&
                    !resource.Queue.Any(q => q.BookedBy.Login == currentUserLogin);
        }

        public bool CanRelease(Resource resource) 
        {
            return !CanBook(resource);
        }

        public Resource Resource { get; private set; }
    }
}
