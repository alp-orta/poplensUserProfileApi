using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using poplensUserProfileApi.Data.Configurations;
using poplensUserProfileApi.Models;

namespace poplensUserProfileApi.Data {
    public class UserProfileDbContext : DbContext {
        public UserProfileDbContext(DbContextOptions<UserProfileDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.HasPostgresExtension("vector");
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

            modelBuilder.Entity<Review>()
                .Property(r => r.Embedding)
                .HasColumnType("vector(384)");

            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new LikeConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql("Host=postgresProfile;Port=5432;Username=postgre;Password=postgre;Database=Profile", o => o.UseVector());

            base.OnConfiguring(optionsBuilder);
        }
    }
}
