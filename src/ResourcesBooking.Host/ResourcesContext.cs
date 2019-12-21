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
        public DbSet<HistoryEntry> History { get; set; }

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
            
            builder.Entity<HistoryEntry>().HasKey(it => it.Id);
            builder.Entity<HistoryEntry>().ToTable("History");
            
            builder.Entity<HistoryEntry>()
                .HasOne(it => it.User)
                .WithMany()
                .HasForeignKey(it => it.UserLogin);

            builder.Entity<HistoryEntry>()
                .Property(it => it.Description)
                .IsRequired();

            builder.Entity<HistoryEntry>()
                .Property(it => it.Date)
                .IsRequired();

            builder.Entity<HistoryEntry>()
                .HasOne(it => it.Resource)
                .WithMany()
                .HasForeignKey(it => it.ResourceId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            base.OnModelCreating(builder);
        }
    }
}