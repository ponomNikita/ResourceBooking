using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResourcesBooking.Host.Commands;
using Serilog;

namespace ResourcesBooking.Host
{
    public class ResourceReleaseBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ResourceReleaseBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Start {@service}", nameof(ResourceReleaseBackgroundService));

            while(!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                try
                {
                    using(var scope = _scopeFactory.CreateScope())
                    {
                        Log.Information("Check for expired reservation");
                        
                        var context = scope.ServiceProvider.GetRequiredService<ResourcesContext>();
                        var mediator = scope.ServiceProvider.GetService<IMediator>();

                        var now = DateTimeOffset.UtcNow;

                        var resourcesToRelease = await context.Resources
                            .WithDetails()
                            .Where(it => it.BookedUntil < now)
                            .ToListAsync(stoppingToken);

                        foreach (var resource in resourcesToRelease)
                        {
                            await mediator.Send(new ReleaseResourceCommand(resource.Id, resource.BookedBy, true));
                        }
                    }
                }
                catch (Exception e)
                {                    
                    Log.Error(e, "Error in {@service}", nameof(ResourceReleaseBackgroundService));
                    throw;
                }
            }
        }
    }
}