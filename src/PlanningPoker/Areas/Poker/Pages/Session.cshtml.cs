using System;
using System.Threading;
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
        // TODO Getting current user
        private User _currentUser = new User
        {
            Login = "nponomarev",
            AvatarUrl = "https://assets.gitlab-static.net/uploads/-/system/user/avatar/1143997/avatar.png"
        };
        
        private readonly IMediator _mediator;

        public SessionModel(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public PokerSession Session { get; set; }
        
        [BindProperty]
        public string SubjectName { get; set; }
        
        [BindProperty]
        public Guid SessionId { get; set; }
        
        [BindProperty]
        public string Vote { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken token)
        {
            await _mediator.Send(new ConnectToSessionByIdCommand(id, _currentUser), token);

            SetPageData(id);
            return Page();
        }

        public async Task<IActionResult> OnPostSubjectAsync(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(SubjectName))
            {
                ModelState.AddModelError(nameof(SubjectName), $"{nameof(SubjectName)} is required");
                SetPageData(SessionId);
                return Page();
            }
            
            await _mediator.Send(new AddSubjectCommand(SubjectName, SessionId), token);
            SetPageData(SessionId);

            return Page();
        }

        public async Task<IActionResult> OnPostVoteAsync(CancellationToken token)
        {
            
            if (string.IsNullOrWhiteSpace(Vote))
            {
                ModelState.AddModelError(nameof(Vote), $"{nameof(Vote)} is required");
                SetPageData(SessionId);
                return Page();
            }
            
            await _mediator.Send(new AddVoteCommand(SessionId, _currentUser, Vote), token);
            SetPageData(SessionId);
            
            return Page();
            
        }

        private void SetPageData(Guid sessionId)
        {
            Session = ActiveSessions.Sessions[sessionId];
            SessionId = sessionId;
            SubjectName = Session.ActiveSubject.Name;
        }

        public bool HasAlreadyVoted()
        {
            return Session.ActiveSubject.HasAlreadyVoted(_currentUser);
        }
    }
}