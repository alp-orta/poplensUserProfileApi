namespace poplensUserProfileApi.Models {
    public class Like {
        public Guid Id { get; set; }
        public Guid ReviewId { get; set; } // FK to the Review table
        public Guid ProfileId { get; set; } // FK to the Profile table
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
