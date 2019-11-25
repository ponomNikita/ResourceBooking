using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ResourcesBooking.Host.BackgroundTasks;
using ResourcesBooking.Host.Options;
using ResourcesBooking.Host.Services;
using Serilog;

namespace ResourcesBooking.Host
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment CurrentEnvironment { get; }     

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddRazorPages();

            services.AddAuthorization(Configuration);

            services.AddDbContext<ResourcesContext>(options => 
                options.UseNpgsql(Configuration.GetConnectionString("Resources")), ServiceLifetime.Scoped);

            services.AddMediatR(Assembly.GetAssembly(GetType()));

            services.AddSingleton(Configuration.GetSection("booking")?.Get<BookingOptions>() ?? new BookingOptions());
            
            var notificationOptions = Configuration.GetSection("notifications")
                ?.Get<NotificationOptions>()
                ?? new NotificationOptions();

            services.AddSingleton(notificationOptions);

            if (notificationOptions.Mattermost != null)
            {
                services.AddScoped<INotificationService, MattermostNotificationService>();
            }
            else if (CurrentEnvironment.IsDevelopment())
            {
                services.AddScoped<INotificationService, DevelopmentNotificationService>();
            }

            services.AddScoped<IBackgroundTask, NotifyUsersAboutEndingOfReservationBackgroundTask>();
            services.AddScoped<IBackgroundTask, ReleaseExpiredReservationsBackgroundTask>();

            services.AddHostedService<DatabaseInitialization>();
            services.AddHostedService<BackgroundTasksExecutor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();

            app.UseMiddleware<AuthMiddleware>();
 
            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            app.UseSerilogRequestLogging();
        }
    }
}
