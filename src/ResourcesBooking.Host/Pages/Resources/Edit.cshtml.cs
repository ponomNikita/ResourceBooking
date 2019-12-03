using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Commands;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class EditModel : PageModel
    {
        private readonly ResourcesContext _context;
        private readonly IMediator _mediator;

        public EditModel(ResourcesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [BindProperty]
        public EditResourceCommand Command { get; set; }

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

            Command = new EditResourceCommand(resource);

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
            
            await _mediator.Send(Command);

            returnUrl = returnUrl ?? Url.Content("~/");
            return Redirect(returnUrl);
        }
    }
}
