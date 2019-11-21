using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResourcesBooking.Host
{
    public static class AuthorizationRegistrationExtensions
    {
        public static void AddAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "GitLab";
                })
                .AddCookie()
                .AddOAuth("GitLab", options =>
                {
                    options.ClientId = configuration["gitlab:clientId"];
                    options.ClientSecret = configuration["gitlab:clientSecret"];
                    options.CallbackPath = new PathString("/login-collback");

                    options.Scope.Add("read_user");
                    options.Scope.Add("openid");

                    options.AuthorizationEndpoint = configuration["gitlab:authEndpoint"];
                    options.TokenEndpoint = configuration["gitlab:tokenEndpoint"];
                    options.UserInformationEndpoint = configuration["gitlab:userInfoEndpoint"];

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
        }
    }
}