namespace poplensUserProfileApi.Models {
    public class Comment {
        public Guid Id { get; set; }
        public Guid ReviewId { get; set; } // FK to the Review table
        public Guid? ParentCommentId { get; set; } 
        public Guid ProfileId { get; set; } // FK to the Profile table
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
