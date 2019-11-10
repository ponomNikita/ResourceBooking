using System.Linq;
using System.Security.Claims;

namespace ResourcesBooking.Host
{
    public static class UserExtensions
    {
        public static string GetAvatarUrl(this ClaimsPrincipal user)
        {
            return user?.Claims.FirstOrDefault(c => c.Type == "urn:gitlab:avatar")?.Value ?? "";
        }
    }
}