using System;
using MediatR;

namespace PlanningPoker.Commands
{
    public class AddSubjectCommand : IRequest
    {
        public AddSubjectCommand(string subjectName, Guid sessionId)
        {
            SubjectName = subjectName;
            SessionId = sessionId;
        }

        public string SubjectName { get; }
        public Guid SessionId { get; }
    }
}