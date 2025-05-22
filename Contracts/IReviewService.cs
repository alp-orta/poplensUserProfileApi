using Pgvector;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Models.Common;
using poplensUserProfileApi.Models.Dtos;
using poplensUserProfileApi.Models.Feed;

namespace poplensUserProfileApi.Contracts {
    public interface IReviewService {
        Task<Review> GetReviewByIdAsync(Guid reviewId);
        Task<ReviewDetail> GetReviewDetailAsync(Guid reviewId, string token);
        Task AddReviewAsync(Guid profileId, CreateReviewRequest request, string token);
        Task<int> UpdateMissingReviewEmbeddingsAsync(string token);
        Task<bool> DeleteReviewAsync(Guid profileId, string mediaId);
        Task<List<Review>> GetReviewsByProfileIdAsync(Guid profileId, int page = 1, int pageSize = 10); 
        Task<List<Review>> GetReviewsByProfileIdsAsync(List<Guid> profileIds, int page = 1, int pageSize = 10);
        Task<MediaMainPageReviewInfo> GetMediaMainPageReviewInfo(string mediaId, string token);
        Task<PageResult<ReviewWithUsername>> GetMediaReviews(string mediaId, int page, int pageSize, string sortOption, string token);
        Task<List<Review>> GetReviewsWithEmbeddingsAsync(Guid profileId);
        Task<List<Review>> GetLikedReviewsWithEmbeddingsAsync(Guid profileId);
        Task<List<Review>> GetCommentedReviewsWithEmbeddingsAsync(Guid profileId);
        Task<List<Review>> GetSimilarReviewsAsync(Vector embedding, int count, List<Guid>? excludedReviewIds = null, Guid? requestingProfileId = null);
        Task<UserInteractionsResponse> GetUserInteractionsWithEmbeddingsAsync(Guid profileId);

        // ────────────────────── Likes ──────────────────────
        Task AddLikeAsync(Guid profileId, Guid reviewId);
        Task RemoveLikeAsync(Guid profileId, Guid reviewId);
        Task<int> GetLikeCountAsync(Guid reviewId);
        Task<bool> HasUserLikedAsync(Guid profileId, Guid reviewId);

        // ────────────────────── Comments ──────────────────────
        Task AddCommentAsync(Guid profileId, Guid reviewId, CreateCommentRequest request);
        Task UpdateCommentAsync(Guid commentId, string newContent);
        Task DeleteCommentAsync(Guid commentId);
        Task<List<CommentDetail>> GetTopLevelCommentsAsync(Guid reviewId, string token);
        Task<List<CommentDetail>> GetRepliesAsync(Guid parentCommentId, string token);
        Task<int> GetCommentCountAsync(Guid reviewId);
        Task<int> GetReplyCountAsync(Guid parentCommentId);
    }
}