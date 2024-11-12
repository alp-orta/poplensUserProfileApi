using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Models;

namespace poplensUserProfileApi.Data {
    // UserProfileDbContext.cs
    public class UserProfileDbContext : DbContext {
        public UserProfileDbContext(DbContextOptions<UserProfileDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Followers)
                .WithOne()
                .HasForeignKey(f => f.FollowingId);

            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Following)
                .WithOne()
                .HasForeignKey(f => f.FollowerId);
            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Reviews) // A Profile has many Reviews
                .WithOne()
                .HasForeignKey(r => r.ProfileId) // The foreign key in the Review entity is ProfileId
                .IsRequired(); // Make sure ProfileId is required (if needed)

            base.OnModelCreating(modelBuilder);
        }
    }

}
