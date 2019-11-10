using System;
using System.ComponentModel.DataAnnotations;

namespace ResourcesBooking.Host.Models
{
    public class BookingTicket
    {
        private BookingTicket() {}

        public BookingTicket(string bookingReason, User bookedBy, long durationInMinutes, DateTimeOffset bookingDate)
        {
            BookingReason = bookingReason;
            BookedBy = bookedBy;
            DurationInMinutes = durationInMinutes;
            BookingDate = bookingDate;
        }

        public Guid Id { get; set;}

        [Display(Name = "Booking reason")]
        public string BookingReason { get; set; }
        
        [Display(Name = "Booked by")]
        public User BookedBy { get; set; }
            
        [Display(Name = "Booking duration in minutes")]
        public long DurationInMinutes { get; set; }

        public DateTimeOffset BookingDate { get; set; }
    }
}