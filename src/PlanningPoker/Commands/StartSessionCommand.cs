using System.ComponentModel.DataAnnotations;
using MediatR;
using PlanningPoker.Models;

namespace PlanningPoker.Commands
{
    public class StartSessionCommand : IRequest<PokerSession>
    {
        [Required]
        public string Name { get; set; }
        
        public string Lead { get; set; }
    }
}