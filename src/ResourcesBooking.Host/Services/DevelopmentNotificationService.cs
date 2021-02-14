using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonLibs;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Services
{
    public class DevelopmentNotificationService : INotificationService
    {
        private const string NotificationsFolder = "notificationsTmp";
        public async Task Notify(User user, string payload, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(NotificationsFolder))
            {
                Directory.CreateDirectory(NotificationsFolder);
            }

            var filename = $"{NotificationsFolder}/Notifications.{user.Login}.txt";

            await File.AppendAllLinesAsync(filename, new string[] { $"[{DateTimeOffset.Now}] {payload}" }, cancellationToken);
        }
    }
}