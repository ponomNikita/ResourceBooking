using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResourcesBooking.Host.Commands;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class AddModel : PageModel
    {
        private readonly DatabaseContext _context;
        private readonly IMediator _mediator;

        public AddModel(DatabaseContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [BindProperty]
        public AddResourceCommand Command { get; set; }

        public string ReturnUrl { get; set; }
        
        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            Command = new AddResourceCommand
            {
                GroupId = id
            };
            
            ReturnUrl = Request.Headers["Referer"].ToString();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() 
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            await _mediator.Send(Command);

            return Redirect(Url.Content("~/"));
        }
    }
}
