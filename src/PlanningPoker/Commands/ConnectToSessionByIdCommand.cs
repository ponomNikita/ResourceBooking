using System;
using MediatR;
using PlanningPoker.Models;

namespace PlanningPoker.Commands
{
    public class ConnectToSessionByIdCommand : IRequest<PokerSession>
    {
        public ConnectToSessionByIdCommand(Guid sessionId, string user)
        {
            SessionId = sessionId;
            User = user;
        }

        public Guid SessionId { get; }
        public string User { get; set; }
    }
}