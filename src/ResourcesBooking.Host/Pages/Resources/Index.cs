using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class IndexModel : PageModel
    {
        private ResourcesContext _context;

        public IndexModel(ResourcesContext context) => _context = context;

        public void OnGet()
        {
            Groups = _context.Groups.WithDetails().ToList();
        }

        public List<ResourcesGroup> Groups { get; private set; }
    }
}
