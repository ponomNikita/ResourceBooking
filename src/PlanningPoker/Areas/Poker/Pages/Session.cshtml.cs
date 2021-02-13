using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PlanningPoker.Commands;
using PlanningPoker.Models;

namespace PlanningPoker
{
    public class SessionModel : PageModel
    {
        private readonly IMediator _mediator;

        public SessionModel(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public PokerSession Session { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var session = await _mediator.Send(new ConnectToSessionByIdCommand(id, User.Identity.Name));

            Session = session;

            return Page();
        }
    }
}