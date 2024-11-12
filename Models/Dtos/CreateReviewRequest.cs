namespace poplensUserProfileApi.Models.Dtos {
    public class CreateReviewRequest {
        public string MediaId { get; set; }
        public string Content { get; set; }
        public float Rating { get; set; }
    }
}
