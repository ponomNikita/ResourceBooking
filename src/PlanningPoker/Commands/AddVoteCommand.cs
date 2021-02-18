using System;
using CommonLibs;
using MediatR;

namespace PlanningPoker.Commands
{
    public class AddVoteCommand : IRequest
    {
        public AddVoteCommand(Guid sessionId, User voter, string vote)
        {
            SessionId = sessionId;
            Voter = voter;
            Vote = vote;
        }
        
        public Guid SessionId { get; }
        public User Voter { get; }
        public string Vote { get; }
    }
}