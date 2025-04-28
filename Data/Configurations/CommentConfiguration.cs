using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Models;

namespace poplensUserProfileApi.Data.Configurations {
    public class CommentConfiguration : IEntityTypeConfiguration<Comment> {
        public void Configure(EntityTypeBuilder<Comment> builder) {
            // Table name
            builder.ToTable("Comments");

            // Primary key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(256); // Limit content size to 256 characters

            builder.Property(c => c.CreatedDate)
                .IsRequired();

            builder.Property(c => c.LastUpdatedDate)
                .IsRequired();

            // Relationships
            builder.HasOne<Review>()
                .WithMany()
                .HasForeignKey(c => c.ReviewId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete to remove comments when the review is deleted

            // Self - referencing relationship for nested comments
            builder.HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to avoid unintended deletions of child comments
        }
    }
}
