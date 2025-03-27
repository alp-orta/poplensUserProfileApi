namespace poplensUserProfileApi.Models {
    public class MediaMainPageReviewInfo {
        public Dictionary<float, float> RatingChartInfo { get; set; }
        public List<ReviewWithUsername> PopularReviews { get; set; }
        public List<ReviewWithUsername> RecentReviews { get; set; }
    }
}
