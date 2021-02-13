using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlanningPoker.Commands;

namespace PlanningPoker
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator = mediator;
        }

        [BindProperty]
        public StartSessionCommand Command { get; set; }
        
        public IActionResult OnGet()
        {
            Command = new StartSessionCommand();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Command.Lead = User.Identity.Name;
            var session = await _mediator.Send(Command);
            
            return RedirectToPage("Session", new {id = session.Id});
        }
        
    }
}
