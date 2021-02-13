using System;
using System.Linq;
using System.Reflection;
using MediatR;
using MediatR.Pipeline;
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
using ResourcesBooking.Host.Commands.Postprocessors;
using Serilog;
using SimpleInjector;

namespace ResourcesBooking.Host
{
    public class Startup
    {
        private readonly Container _container = new Container();

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = environment;
        }

        public Startup(IConfiguration configuration, IWebHostEnvironment currentEnvironment)
        {
            this.Configuration = configuration;
            this.CurrentEnvironment = currentEnvironment;

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

            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore()
                    .AddControllerActivation()
                    .AddViewComponentActivation()
                    .AddPageModelActivation()
                    .AddTagHelperActivation();

                options.AddLogging();
            });

            ConfigureContainer();

            services.AddSingleton<IHostedService>(
                new DatabaseInitialization(_container)); 
            services.AddSingleton<IHostedService>(
                new BackgroundTasksExecutor(_container));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSimpleInjector(_container);

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            app.UseSerilogRequestLogging();

            _container.Verify();
        }

        private void ConfigureContainer()
        {
            AddMediatR();

            _container.RegisterInstance(Configuration.GetSection("booking")?.Get<BookingOptions>() ?? new BookingOptions());

            var notificationOptions = Configuration.GetSection("notifications")
                ?.Get<NotificationOptions>()
                ?? new NotificationOptions();

            _container.RegisterInstance(notificationOptions);

            if (notificationOptions.Mattermost != null)
            {
                _container.Register<INotificationService, MattermostNotificationService>(Lifestyle.Scoped);
            }
            else
            {
                _container.Register<INotificationService, DevelopmentNotificationService>(Lifestyle.Scoped);
            }

            _container.Register<ResourcesContext>(Lifestyle.Scoped);
            _container.Register<IHttpContextAccessor, HttpContextAccessor>(Lifestyle.Scoped);

            _container.Collection.Register(typeof(IBackgroundTask), new []
            {
                typeof(NotifyUsersAboutEndingOfReservationBackgroundTask),
                typeof(ReleaseExpiredReservationsBackgroundTask)
            });
            
            _container.Collection.Append(typeof(IHostedService),
                Lifestyle.Singleton.CreateRegistration(typeof(DatabaseInitialization), _container));
            _container.Collection.Append(typeof(IHostedService),
                Lifestyle.Singleton.CreateRegistration(typeof(BackgroundTasksExecutor), _container));
        }

        private void AddMediatR()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            _container.RegisterSingleton<IMediator, Mediator>();
            
            _container.Register(typeof(IRequestHandler<,>), assemblies);
            _container.Collection.Register(typeof(INotificationHandler<>), assemblies);

            _container.Collection.Register(typeof(IPipelineBehavior<,>), new []
            {
                typeof(RequestPreProcessorBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>)
            });

            _container.Collection.Register(typeof(IRequestPreProcessor<>));
            _container.Collection.Register(typeof(IRequestPostProcessor<,>), new []
            {
                // order is important
                typeof(ReleaseResourcePostProcessor),
                typeof(BookResourcePostProcessor),
                typeof(EditResourcePostProcessor),
                typeof(UnitOfWorkPipelineBehavior<,>),
                typeof(NotificateOnReleaseResourcePostProcessor)
            });

            _container.RegisterInstance(new ServiceFactory(_container.GetInstance));

        }
    }
}
