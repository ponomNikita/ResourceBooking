namespace ResourcesBooking.Host.Options
{
    public class NotificationOptions
    {
        public string Hostname { get; set; }

        public int NotifyBeforeEndingOfReservationInMinutes { get; set; } = 10;
        public Mattermost Mattermost { get; set; }
    }

    public class Mattermost
    {
        public string Hook { get; set; }
    }
}