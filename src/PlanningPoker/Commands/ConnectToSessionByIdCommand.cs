using System;
using CommonLibs;
using MediatR;
using PlanningPoker.Models;

namespace PlanningPoker.Commands
{
    public class ConnectToSessionByIdCommand : IRequest<PokerSession>
    {
        public ConnectToSessionByIdCommand(Guid sessionId, User user)
        {
            SessionId = sessionId;
            User = user;
        }

        public Guid SessionId { get; }
        public User User { get; set; }
    }
}