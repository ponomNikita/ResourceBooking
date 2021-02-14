using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Commands.Postprocessors
{
    public class EditResourcePostProcessor : IRequestPostProcessor<EditResourceCommand, Unit>
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EditResourcePostProcessor(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Process(EditResourceCommand command, Unit response, CancellationToken cancellationToken)
        {
            var history = HistoryEntry.Create(Guid.NewGuid(), 
                "Resource was updated",
                _httpContextAccessor.HttpContext.User.Identity.Name,
                command.Id,
                DateTimeOffset.UtcNow);

            await _context.AddAsync(history);
        }
    }
}