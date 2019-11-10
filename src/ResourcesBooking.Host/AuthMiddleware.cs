using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ResourcesBooking.Host
{
    public class AuthMiddleware
    {
        private static readonly List<string> _unauthorizedAccessAllowPaths = new List<string>
        {
            "login"
        };
        
        private readonly RequestDelegate _next;
 
        public AuthMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
    
        public async Task InvokeAsync(HttpContext context)
        {
            if (!IsExcludedPath(context.Request.Path) &&
                context.User?.Identity?.IsAuthenticated == false)
            {
                context.Response.Redirect("account/login");          
            }

            await _next(context);
        }

        private static bool IsExcludedPath(string path) 
        {
            return _unauthorizedAccessAllowPaths.Any(p => 
                path.Contains(p, 
                System.StringComparison.InvariantCultureIgnoreCase));
        }
    }
}