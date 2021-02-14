using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibs;

namespace PlanningPoker.Models
{
    public class Subject
    {
        public Subject(Guid id, string name, bool isActive, IEnumerable<User> voters)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
            Voters = voters.Select(v => new Voter(v)).ToHashSet();
        }
        
        public Guid Id { get; set; }
        public string Name { get; set;  }
        public bool IsActive { get; set; }
        public HashSet<Voter> Voters { get; set; }

        public void AddVoter(User user)
        {
            Voters.Add(new Voter(user));
        }

        public void Vote(User user, string result)
        {
            var voter = Voters.FirstOrDefault(it => it.User.Login == user.Login);
            if (voter == null)
            {
                throw new Exception($"Voter {user.Login} not registered");
            }

            if (voter.Result != null)
            {
                throw new Exception($"Voter {user.Login} has already voted");
            }

            voter.Result = result;
        }
    }

    public class Voter
    {
        public Voter(User user)
        {
            User = user;
        }

        public User User { get; }
        
        public string Result { get; set; }

        public override int GetHashCode()
        {
            return User.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Voter voter &&
                   User.Equals(voter.User);
        }
    }
}