using System.Collections.Generic;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class HistoryPartialModel
    {
        public IEnumerable<HistoryEntry> History { get; set; }
    }
}