using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Models;

namespace poplensUserProfileApi.Data {
    public class UserProfileDbContext : DbContext {
        public UserProfileDbContext(DbContextOptions<UserProfileDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            // Specify the schema
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Followers)
                .WithOne()
                .HasForeignKey(f => f.FollowingId);

            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Following)
                .WithOne()
                .HasForeignKey(f => f.FollowerId);

            modelBuilder.Entity<Profile>()
                .HasMany(p => p.Reviews)
                .WithOne()
                .HasForeignKey(r => r.ProfileId)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
