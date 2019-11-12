namespace ResourcesBooking.Host.Options
{
    public class NotificationOptions
    {
        public string Hostname { get; set; }
        public Mattermost Mattermost { get; set; }
    }

    public class Mattermost
    {
        public string Hook { get; set; }
    }
}