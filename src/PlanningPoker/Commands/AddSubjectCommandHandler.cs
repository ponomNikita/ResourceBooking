using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace PlanningPoker.Commands
{
    public class AddSubjectCommandHandler : IRequestHandler<AddSubjectCommand, Unit>
    {
        public Task<Unit> Handle(AddSubjectCommand command, CancellationToken cancellationToken)
        {
            var session = ActiveSessions.Sessions[command.SessionId];
            
            session.AddSubject(command.SubjectName);

            return Task.FromResult(Unit.Value);
        }
    }
}