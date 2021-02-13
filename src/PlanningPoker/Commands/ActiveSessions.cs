using System;
using System.Collections.Concurrent;
using PlanningPoker.Models;

namespace PlanningPoker.Commands
{
    // TODO Switch to postgres persistence
    public static class ActiveSessions
    {
        public static ConcurrentDictionary<Guid, PokerSession> Sessions { get; set; } =
            new ConcurrentDictionary<Guid, PokerSession>();
    }
}