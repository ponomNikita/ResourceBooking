using System;
using System.Threading.Tasks;
using CommonLibs;
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
            var user = new User
            {
                Login = "nponomarev",
                AvatarUrl = "https://assets.gitlab-static.net/uploads/-/system/user/avatar/1143997/avatar.png"
            };
            
            var session = await _mediator.Send(new ConnectToSessionByIdCommand(id, user));

            Session = session;

            return Page();
        }
    }
}