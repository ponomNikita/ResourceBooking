using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace PlanningPoker.Commands
{
    public class AddVoteCommandHandler : IRequestHandler<AddVoteCommand, Unit>
    {
        public Task<Unit> Handle(AddVoteCommand command, CancellationToken cancellationToken)
        {
            var session = ActiveSessions.Sessions[command.SessionId];
            session.Vote(command.Voter, command.Vote);

            return Task.FromResult(Unit.Value);
        }
    }
}