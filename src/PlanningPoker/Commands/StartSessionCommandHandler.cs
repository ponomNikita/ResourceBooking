using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using PlanningPoker.Models;

namespace PlanningPoker.Commands
{
    public class StartSessionCommandHandler : IRequestHandler<StartSessionCommand, PokerSession>
    {
        public async Task<PokerSession> Handle(StartSessionCommand command, CancellationToken cancellationToken)
        {
            var session = new PokerSession(Guid.NewGuid(), command.Name, command.Lead, new CardsSet
            {
                Cards = new List<string>
                {
                    "S", "M", "L", "XL"
                }
            });

            ActiveSessions.Sessions.TryAdd(session.Id, session);
                
            return session;
        }
    }
}