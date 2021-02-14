using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommonLibs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using ResourcesBooking.Host.Models;
using Serilog;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace ResourcesBooking.Host
{
    public class DatabaseInitialization : BackgroundService
    {
        private const string DataSeededSettingKey = "DataSeeded";
        private const string SystemUserSettingKey = "SystemUserSeeded";
        private readonly Container _container;

        public DatabaseInitialization(Container container)
        {
            _container = container;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("Migrating database");

            using(AsyncScopedLifestyle.BeginScope(_container))
            {
                var context = _container.GetService<DatabaseContext>();
            
                await context.Database.MigrateAsync(stoppingToken);

                if ((await context.GetSetting(DataSeededSettingKey)) != "true")
                {
                    await Seed(context, stoppingToken);
                }

                if ((await context.GetSetting(SystemUserSettingKey)) != "true")
                {
                    await AddSystemUser(context, stoppingToken);
                }
            }

            Log.Information("Finished migrating database");
        }

        private async Task AddSystemUser(DatabaseContext context, CancellationToken token)
        {
            await context.Users.AddAsync(User.GetSystemUser(), token);
            await context.AddOrUpdateSetting(SystemUserSettingKey, "true");
            await context.SaveChangesAsync(token);
        }

        private async Task Seed(DatabaseContext context, CancellationToken token)
        {
            var resourcesPath  = Path.GetFullPath("data/resources.json");
            if (File.Exists(resourcesPath))
            {                
                var json = JObject.Parse(System.IO.File.ReadAllText(resourcesPath));
                var groups = json.SelectToken("groups").Select(g => g.ToObject<Models.ResourcesGroup>()).ToList();

                var existingGroups = context.Groups.Include(it => it.Resources).ToList();
                var existingResources = existingGroups.SelectMany(it => it.Resources).ToList();
                
                foreach(var group in groups)
                {
                    if (!existingGroups.Contains(group)) 
                    {
                        Log.Information("Seeding data: add group {@group}", group.Name);
                        await context.AddAsync(group, token);
                    }

                    if (group.Resources != null)
                    {
                        foreach (var resource in group.Resources)
                        {
                            if (!existingResources.Contains(resource)) 
                            {
                                Log.Information("Seeding data: add resource {@resource}", resource.Name);
                                await context.AddAsync(resource, token);
                            }
                        }
                    }
                }

                await context.AddOrUpdateSetting(DataSeededSettingKey, "true");
                await context.SaveChangesAsync(token);
            }
        }
    }
}