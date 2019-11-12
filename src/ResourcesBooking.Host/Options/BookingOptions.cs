namespace ResourcesBooking.Host.Options
{
    public class BookingOptions
    {
        public int MaxBookingPeriodInMinutes { get; set; } = 1440;
        
        public int MinBookingPeriodInMinutes { get; set; } = 20;
    }
}