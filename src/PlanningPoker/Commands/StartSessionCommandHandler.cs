using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonLibs;
using MediatR;
using PlanningPoker.Models;

namespace PlanningPoker.Commands
{
    public class StartSessionCommandHandler : IRequestHandler<StartSessionCommand, PokerSession>
    {
        public async Task<PokerSession> Handle(StartSessionCommand command, CancellationToken cancellationToken)
        {
            // TODO remove debug data seed
            
            var user = User.GetSystemUser();
            var session = new PokerSession(Guid.NewGuid(), command.Name, user, new CardsSet
            {
                Cards = new List<string>
                {
                    "S", "M", "L", "XL"
                }
            });
            
            session.AddParticipant(user);
            session.AddSubject("subject 1");
            session.Vote(user, "L");
            session.AddSubject("subject 2");
            session.Vote(user, "XL");
            session.AddSubject("subject 3");
            session.Vote(user, "M");
            session.AddSubject("subject 4");
            session.Vote(user, "S");
            session.AddSubject("subject 5");
            session.Vote(user, "S");
            session.AddSubject("subject 6");
            session.Vote(user, "S");
            session.AddSubject("subject 7");
            session.Vote(user, "S");
            session.AddSubject("subject 8");
            session.Vote(user, "S");
            session.AddSubject("subject 9");
            session.Vote(user, "S");
            session.AddSubject("subject 10");
            session.Vote(user, "S");
            session.AddSubject("subject 11");
            session.Vote(user, "S");
            session.AddSubject("subject 12");
            session.Vote(user, "S");

            ActiveSessions.Sessions.TryAdd(session.Id, session);
                
            return session;
        }
    }
}