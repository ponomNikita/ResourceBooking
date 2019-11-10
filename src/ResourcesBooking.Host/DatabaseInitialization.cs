using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Serilog;

namespace ResourcesBooking.Host
{
    public class DatabaseInitialization : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DatabaseInitialization(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Migrating database");

            using(var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ResourcesContext>();
            
                await context.Database.MigrateAsync(stoppingToken);

                await Seed(context, stoppingToken);
            }

            Log.Information("Finished migrating database");
        }

        private async Task Seed(ResourcesContext context, CancellationToken token)
        {            
            var json = JObject.Parse(System.IO.File.ReadAllText("resources.json"));
            var groups = json.SelectToken("groups").Select(g => g.ToObject<Models.ResourcesGroup>()).ToList();

            var existingGroups = context.Groups.Include(it => it.Resources).ToList();
            var existingResources = existingGroups.SelectMany(it => it.Resources).ToList();
            
            foreach(var group in groups)
            {
                if (!existingGroups.Contains(group)) 
                {
                    await context.AddAsync(group, token);
                }

                if (group.Resources != null)
                {
                    foreach (var resource in group.Resources)
                    {
                        if (!existingResources.Contains(resource)) 
                        {
                            await context.AddAsync(resource, token);
                        }
                    }
                }
            }

            await context.SaveChangesAsync(token);
        }
    }
}