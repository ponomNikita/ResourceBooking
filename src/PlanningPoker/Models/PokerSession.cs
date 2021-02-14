using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibs;

namespace PlanningPoker.Models
{
    public class PokerSession
    {
        private List<Subject> _subjects = new List<Subject>();
        private HashSet<User> _participants = new HashSet<User>();

        public PokerSession(Guid id, string name, User lead, CardsSet cardsSet)
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
        public User Lead { get; set; }

        /// <summary>
        /// Poker participants
        /// </summary>
        public IReadOnlyCollection<User> Participants => _participants;

        /// <summary>
        /// Set of poker cards
        /// </summary>
        public CardsSet CardsSet { get; set; }

        public IReadOnlyCollection<Subject> Subjects => _subjects;

        public Subject ActiveSubject => Subjects.FirstOrDefault(s => s.IsActive);

        public void AddSubject(string name)
        {
            _subjects.ForEach(s => s.IsActive = false);
            _subjects.Add(new Subject(Guid.NewGuid(), name, true, Participants));
        }

        public void AddParticipant(User user)
        {
            _participants.Add(user);

            ActiveSubject?.AddVoter(user);
        }

        public void Vote(User user, string result)
        {
            ActiveSubject?.Vote(user, result);
        }
    }
}