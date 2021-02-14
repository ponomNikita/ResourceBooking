using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host.Pages.Resources
{
    public class IndexModel : PageModel
    {
        private DatabaseContext _context;

        public IndexModel(DatabaseContext context) => _context = context;

        public void OnGet()
        {
            Groups = _context.Groups.WithDetails().ToList();
        }

        public List<ResourcesGroup> Groups { get; private set; }

        public string GetShortDescription(Resource resource)
        {
            if (resource?.Description == null)
            {
                return string.Empty;
            }

            if(resource.Description.Length <= 20)
            {
                return resource.Description;
            }
            else
            {
                return $"{resource.Description.Substring(0, 20)}...";
            }
        }
    }
}
