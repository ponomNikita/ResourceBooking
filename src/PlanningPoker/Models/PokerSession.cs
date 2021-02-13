using System;
using System.Collections.Generic;

namespace PlanningPoker.Models
{
    public class PokerSession
    {
        public PokerSession(Guid id, string name, string lead, CardsSet cardsSet)
        {
            Id = id;
            Name = name;
            Lead = lead;
            CardsSet = cardsSet;
        }
        
        public Guid Id { get; set; }
        
        /// <summary>
        /// Name of poker session
        /// </summary>
        public string Name { get; set; }
        
        //TODO Must be User, not string
        /// <summary>
        /// Poker lead user
        /// </summary>
        public string Lead { get; set; }

        /// <summary>
        /// Poker participants
        /// </summary>
        public HashSet<string> Participants { get; set; } = new HashSet<string>();
        
        /// <summary>
        /// Set of poker cards
        /// </summary>
        public CardsSet CardsSet { get; set; }

        public List<Subject> Subjects { get; set; } = new List<Subject>();
    }
}