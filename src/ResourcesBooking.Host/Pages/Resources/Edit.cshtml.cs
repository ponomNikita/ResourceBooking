using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Commands;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class EditModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IMediator _mediator;

        public EditModel(DatabaseContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [BindProperty]
        public EditResourceCommand Command { get; set; }

        public string ReturnUrl { get; set; }

        public bool IsGroup { get; set; }
        
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var resource = await OnGet(id);

            if (resource == null) 
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null) 
        {
            if (!ModelState.IsValid)
            {
                await OnGet(Command.Id, returnUrl);
                return Page();
            }
            
            await _mediator.Send(Command);

            returnUrl = returnUrl ?? Url.Content("~/");
            return Redirect(returnUrl);
        }

        public async Task<IActionResult> OnPostRemoveAsync(Guid id, string returnUrl = null)
        {
            await _mediator.Send(new RemoveResourceCommand(id));

            return Redirect(Url.Content("~/"));
        }

        private async Task<Resource> OnGet(Guid id, string returnUrl = null)
        {
            var resource = await _context.Resources
                .WithDetails()
                .FirstOrDefaultAsync(it => it.Id == id);

            Command = new EditResourceCommand(resource);

            ReturnUrl = returnUrl ?? Request.Headers["Referer"].ToString();

            IsGroup = resource is ResourcesGroup;

            return resource;
        }
    }
}
