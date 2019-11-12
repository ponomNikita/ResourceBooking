using System;
using System.Threading;
using System.Threading.Tasks;
using Matterhook.NET.MatterhookClient;
using ResourcesBooking.Host.Models;
using ResourcesBooking.Host.Options;
using Serilog;

namespace ResourcesBooking.Host.Services
{
    public class MattermostNotificationService : INotificationService
    {
        private readonly MatterhookClient _clinet;
        private readonly NotificationOptions _options;

        public MattermostNotificationService(NotificationOptions options)
        {
            if (string.IsNullOrWhiteSpace(options?.Mattermost?.Hook))
                throw new ArgumentNullException(nameof(options.Mattermost.Hook));

            _clinet = new MatterhookClient(options.Mattermost.Hook);
            _options = options;
        }

        public async Task Notify(User user, string payload, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Sending notification for user {@user}", user.Login);

                var response = await _clinet.PostAsync(new MattermostMessage
                {
                    Channel = $"@{user.Login}",
                    Text = payload,
                    Username = "ResourceBooking"
                });

                if (!response.IsSuccessStatusCode)
                {
                    Log.Error($"Failed to send notification for user {user.Login}. Status code {response.StatusCode}");
                }
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to send notification for user {user.Login}");
                throw;
            }
        }
    }
}