using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host
{
    public static class ResourcesContextExtensions
    {
        public static IQueryable<Resource> WithDetails(this DbSet<Resource> dbSet)
        {
            return dbSet
                .Include(it => it.BookedBy)
                .Include(it => it.Queue)
                    .ThenInclude(it => it.BookedBy);
        }

        public static IQueryable<ResourcesGroup> WithDetails(this DbSet<ResourcesGroup> dbSet)
        {
            return dbSet
                .Include(it => it.Resources)
                    .ThenInclude(it => it.BookedBy);
        }

        public static async Task<User> GetOrAdd(this ResourcesContext context, ClaimsPrincipal currentUser) 
        {            
            var user = await context.Users.FindAsync(currentUser.Identity.Name);
            var avatar = currentUser.GetAvatarUrl();

            if (user == null)
            {
                user = new User
                {
                    Login = currentUser.Identity.Name,
                    AvatarUrl = currentUser.GetAvatarUrl()
                };

                await context.AddAsync(user);
            }
            else if (user.AvatarUrl != avatar)
            {
                user.AvatarUrl = avatar;
            }
            
            return user;
        }

        public static async Task AddOrUpdateSetting(this ResourcesContext context, string key, string value)
        {
            var settingValue = await context.Settings.FirstOrDefaultAsync(it => it.Key == key);

            if (settingValue == null)
            {
                await context.Settings.AddAsync(new KeyValue 
                {
                    Key = key,
                    Value = value
                });
            }
            else
            {
                settingValue.Value = value;
            }
        }

        public static async Task<string> GetSetting(this ResourcesContext context, string key)
        {
            var setting = await context.Settings.FirstOrDefaultAsync(it => it.Key == key);
            return setting?.Value;
        }
    }
}