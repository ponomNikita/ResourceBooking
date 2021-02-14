using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;

namespace ResourcesBooking.Host.Commands.Postprocessors
{
    public class UnitOfWorkPipelineBehavior<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    {
        private readonly DatabaseContext _context;

        public UnitOfWorkPipelineBehavior(DatabaseContext context)
        {
            _context = context;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (request is IRequireSaveChanges)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}