using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Matterhook.NET.MatterhookClient;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "GitLab";
                })
                .AddCookie()
                .AddOAuth("GitLab", options =>
                {
                    options.ClientId = Configuration["gitlab:clientId"];
                    options.ClientSecret = Configuration["gitlab:clientSecret"];
                    options.CallbackPath = new PathString("/login-collback");

                    options.Scope.Add("read_user");
                    options.Scope.Add("openid");

                    options.AuthorizationEndpoint = Configuration["gitlab:authEndpoint"];
                    options.TokenEndpoint = Configuration["gitlab:tokenEndpoint"];
                    options.UserInformationEndpoint = Configuration["gitlab:userInfoEndpoint"];

                    options.SaveTokens = true;

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "username");
                    options.ClaimActions.MapJsonKey("urn:gitlab:url", "html_url");
                    options.ClaimActions.MapJsonKey("urn:gitlab:avatar", "avatar_url");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                            response.EnsureSuccessStatusCode();

                            var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(user.RootElement);
                        }
                    };
                });

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
            
            if (CurrentEnvironment.IsDevelopment())
            {
                services.AddScoped<INotificationService, DevelopmentNotificationService>();
            }

            services.AddHostedService<DatabaseInitialization>();
            services.AddHostedService<ResourceReleaseBackgroundService>();
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
