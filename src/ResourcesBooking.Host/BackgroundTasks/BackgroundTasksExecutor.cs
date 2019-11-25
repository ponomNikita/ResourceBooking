using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ResourcesBooking.Host.BackgroundTasks
{
    public class BackgroundTasksExecutor : BackgroundService
    {
        private const int BackgroundServicePeriodInSeconds = 60;
        private readonly IServiceScopeFactory _scopeFactory;

        public BackgroundTasksExecutor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Start {@service}", nameof(BackgroundTasksExecutor));

            while(!stoppingToken.IsCancellationRequested)
            {
                
                using(var scope = _scopeFactory.CreateScope())
                {                        
                    var backgroundTasks = scope.ServiceProvider.GetServices<IBackgroundTask>();

                    foreach (var task in backgroundTasks)
                    {
                        try
                        {
                            await task.Execute(stoppingToken);                            
                        }
                        catch (Exception e)
                        {         
                            Log.Warning(e, "Error in {@service}", task.GetType().Name);
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(BackgroundServicePeriodInSeconds), stoppingToken);
            }
        }
    }
}