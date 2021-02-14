using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PlanningPoker.Models;

namespace PlanningPoker.Commands
{
    public class ConnectToSessionByIdCommandHandler : IRequestHandler<ConnectToSessionByIdCommand, PokerSession>
    {
        public async Task<PokerSession> Handle(ConnectToSessionByIdCommand command, CancellationToken cancellationToken)
        {
            if (ActiveSessions.Sessions.TryGetValue(command.SessionId, out var session))
            {
                session.AddParticipant(command.User);
                return session;
            }
            
            // TODO NotFoundException
            throw new Exception("Session not found");
        }
    }
}