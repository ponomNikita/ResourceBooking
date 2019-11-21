using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ResourcesBooking.Host.Models
{
    public class Resource
    {        
        public Guid Id { get; set; }
        
        [Display(Name = "Resource name")]
        public string Name { get; set; }

        [Display(Name = "Booked until")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset? BookedUntil { get; set; }

        [Display(Name = "Booking reason")]
        public string BookingReason { get; set; }
        
        [Display(Name = "Booked by")]
        public User BookedBy { get; set; }

        public List<BookingTicket> Queue { get; set;} = new List<BookingTicket>();

        public void Book(User bookedBy, long bookingDurationInMinutes, string bookingReason)
        {
            if (bookedBy.Equals(BookedBy) || Queue.Any(it => it.BookedBy.Equals(bookedBy)))
            {
                throw new ArgumentException("User has already booked this resource");
            }

            if (BookedBy == null)
            {
                BookedBy = bookedBy ?? throw new ArgumentNullException(nameof(bookedBy));
                BookingReason = bookingReason ?? throw new ArgumentNullException(nameof(bookingReason));
                BookedUntil = DateTimeOffset.UtcNow.AddMinutes(bookingDurationInMinutes);
            }
            else
            {
                Queue.Add(new BookingTicket(bookingReason, bookedBy, bookingDurationInMinutes, DateTimeOffset.UtcNow));
            }
        }

        public void Extend(long extendTimeInMinutes)
        {
            if (!BookedUntil.HasValue)
            {
                throw new Exception("Could not extend resource, which had not been booked");
            }

            BookedUntil = BookedUntil.Value.AddMinutes(extendTimeInMinutes);
        }

        public void Release(User user)
        {
           var queueItem = Queue.FirstOrDefault(it => it.BookedBy.Equals(user));

            if (BookedBy.Equals(user))
            {
                if (Queue.Count == 0)
                {
                    BookedUntil = null;
                    BookingReason = null;
                    BookedBy = null;
                }
                else
                {
                    var ticket = Queue.OrderBy(it => it.BookingDate).First();
                    Queue.Remove(ticket);

                    BookingReason = ticket.BookingReason;
                    BookedBy = ticket.BookedBy;
                    BookedUntil = DateTimeOffset.UtcNow.AddMinutes(ticket.DurationInMinutes);

                }
            }
            else if (queueItem != null)
            {
                    Queue.Remove(queueItem);
            }
            else
            {
                throw new ArgumentException($"User {user.Login} is not allowd to release resource because {user.Login} did not book it");
            }
        }

        public DateTimeOffset GetGeneralBookedUntil()
        {
            var result = BookedUntil ?? DateTimeOffset.UtcNow;

            foreach (var item in Queue)
            {
                result.AddMinutes(item.DurationInMinutes);
            }

            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is Resource resource &&
                   Name == resource.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}