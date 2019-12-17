using Microsoft.EntityFrameworkCore;
using ResourcesBooking.Host.Models;

namespace ResourcesBooking.Host
{
    public class ResourcesContext : DbContext
    {
        public ResourcesContext (DbContextOptions options) 
            : base(options)
        {
            
        }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourcesGroup> Groups { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<KeyValue> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Resource>().HasKey(it => it.Id);

            builder.Entity<Resource>()
                .HasMany(it => it.Queue)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BookingTicket>().HasKey(it => it.Id);
            builder.Entity<BookingTicket>().ToTable("BookingQueue");

            builder.Entity<ResourcesGroup>()
                .Property(it => it.Name)
                .IsRequired();

            builder.Entity<ResourcesGroup>()
                .HasMany(it => it.Resources)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>().HasKey(it => it.Login);
            
            builder.Entity<User>()
                .Property(it => it.AvatarUrl)
                .IsRequired();
            
            builder.Entity<KeyValue>().HasKey(it => it.Key);
            builder.Entity<KeyValue>().ToTable("Settings");

            base.OnModelCreating(builder);
        }
    }
}