using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace ResourcesBooking.Host.BackgroundTasks
{
    public class BackgroundTasksExecutor : BackgroundService
    {
        private const int BackgroundServicePeriodInSeconds = 60;
        private readonly Container _container;

        public BackgroundTasksExecutor(Container container)
        {
            _container = container;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Start {@service}", nameof(BackgroundTasksExecutor));

            while(!stoppingToken.IsCancellationRequested)
            {                
                using(AsyncScopedLifestyle.BeginScope(_container))
                {                        
                    var backgroundTasks = _container.GetServices<IBackgroundTask>();

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