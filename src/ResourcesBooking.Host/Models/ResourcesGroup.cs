using System.Collections.Generic;

namespace ResourcesBooking.Host.Models
{
    public class ResourcesGroup : Resource
    {
        public List<Resource> Resources { get; set; }

        public bool AllowToBook { get; set; }
    }
}