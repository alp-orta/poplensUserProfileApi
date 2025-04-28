using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Models;

namespace poplensUserProfileApi.Data.Configurations {
    public class LikeConfiguration : IEntityTypeConfiguration<Like> {
        public void Configure(EntityTypeBuilder<Like> builder) {
            // Table name
            builder.ToTable("Likes");

            // Primary key
            builder.HasKey(l => l.Id);

            // Properties
            builder.Property(l => l.CreatedDate)
                .IsRequired();

            // Relationships
            builder.HasOne<Review>()
                .WithMany()
                .HasForeignKey(l => l.ReviewId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete to remove likes when the review is deleted

            // Unique constraint to prevent duplicate likes for the same review by the same profile
            builder.HasIndex(l => new { l.ReviewId, l.ProfileId })
                .IsUnique();
        }
    }
}
