using System.ComponentModel.DataAnnotations;

namespace ResourcesBooking.Host.Models
{
    public class KeyValue
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}