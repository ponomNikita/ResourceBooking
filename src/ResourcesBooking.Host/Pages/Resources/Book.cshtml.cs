using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Commands;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class BookModel : PageModel
    {
        private readonly ResourcesContext _context;
        private readonly IMediator _mediator;
        private readonly BookingOptions _bookingOptions;

        public BookModel(ResourcesContext context, IMediator mediator, BookingOptions bookingOptions)
        {
            _context = context;
            _mediator = mediator;
            _bookingOptions = bookingOptions;
        }

        [BindProperty]
        public BookResourceRequest BookingRequest { get; set; }

        public string ReturnUrl { get; set; }
        
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == id);

            if (resource == null) 
            {
                return NotFound();
            }

            BookingRequest = new BookResourceRequest
            {                    
                ResourceName = resource.Name,
                ResourceId = resource.Id,
                MaxBookingPeriodInMinutes = _bookingOptions.MaxBookingPeriodInMinutes,
                MinBookingPeriodInMinutes = _bookingOptions.MinBookingPeriodInMinutes,
                BookingPeriodInMinutes = _bookingOptions.MinBookingPeriodInMinutes
            };

            ReturnUrl = Request.Headers["Referer"].ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) 
        {
            if (!ModelState.IsValid)
            {
                ReturnUrl = returnUrl;
                return Page();
            }

            var user = await _context.GetOrAdd(User);

            await _mediator.Send(new BookResourceCommand(
                BookingRequest.ResourceId, 
                user,
                BookingRequest.BookingReason,
                BookingRequest.BookingPeriodInMinutes));

            returnUrl = returnUrl ?? Url.Content("~/");
            return Redirect(returnUrl);
        }
    }

    public class BookResourceRequest 
    {
        [Required]
        public int BookingPeriodInMinutes { get; set; }

        [Required]
        public string BookingReason { get; set; }

        public string ResourceName { get; set; }

        public Guid ResourceId { get; set; }
        
        public int MaxBookingPeriodInMinutes { get; set; }
        
        public int MinBookingPeriodInMinutes { get; set; }
    }
}
