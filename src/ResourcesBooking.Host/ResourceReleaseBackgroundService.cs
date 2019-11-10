using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResourcesBooking.Host.Services;
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
                try
                {
                    using(var scope = _scopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<ResourcesContext>();
                        var bookingService = scope.ServiceProvider.GetService<IBookingService>();

                        var now = DateTimeOffset.UtcNow;

                        var resourcesToRelease = await context.Resources
                            .WithDetails()
                            .Where(it => it.BookedUntil < now)
                            .ToListAsync(stoppingToken);

                        foreach (var resource in resourcesToRelease)
                        {
                            await bookingService.Release(new ReleaseModel(resource.Id, resource.BookedBy));
                        }
                    }
                }
                catch (Exception e)
                {                    
                    Log.Error(e, "Error in {@service}", nameof(ResourceReleaseBackgroundService));
                    throw;
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}