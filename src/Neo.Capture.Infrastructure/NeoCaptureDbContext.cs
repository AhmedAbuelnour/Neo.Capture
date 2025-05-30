using Microsoft.EntityFrameworkCore;
using Neo.Capture.Domain.Entities;

namespace Neo.Capture.Infrastructure
{
    public class NeoCaptureDbContext(DbContextOptions<NeoCaptureDbContext> options) : DbContext(options)
    {
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<ProfileLocation> ProfileLocations { get; set; }
        public DbSet<CheckInLocation> CheckInLocations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CheckInLocation>()
                         .Property(c => c.ImageUrls)
                         .HasColumnType("text[]");

            modelBuilder.Entity<Profile>()
                        .HasMany(p => p.Locations)
                        .WithOne(l => l.Profile)
                        .HasForeignKey(l => l.ProfileId)
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProfileLocation>()
                        .HasOne(l => l.CheckIn)
                        .WithOne(c => c.ProfileLocation)
                        .HasForeignKey<CheckInLocation>(c => c.ProfileLocationId)
                        .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
