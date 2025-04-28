using poplensUserProfileApi.Models;
using poplensUserProfileApi.Models.Common;
using poplensUserProfileApi.Models.Dtos;

namespace poplensUserProfileApi.Contracts {
    public interface IReviewService {
        Task AddReviewAsync(Guid profileId, CreateReviewRequest request, string token);
        Task<bool> DeleteReviewAsync(Guid profileId, string mediaId);
        Task<List<Review>> GetReviewsByProfileIdAsync(Guid profileId, int page = 1, int pageSize = 10); 
        Task<List<Review>> GetReviewsByProfileIdsAsync(List<Guid> profileIds, int page = 1, int pageSize = 10);
        Task<MediaMainPageReviewInfo> GetMediaMainPageReviewInfo(string mediaId, string token);
        Task<PageResult<ReviewWithUsername>> GetMediaReviews(string mediaId, int page, int pageSize, string sortOption, string token);
        // ────────────────────── Likes ──────────────────────
        Task AddLikeAsync(Guid profileId, Guid reviewId);
        Task RemoveLikeAsync(Guid profileId, Guid reviewId);
        Task<int> GetLikeCountAsync(Guid reviewId);
        Task<bool> HasUserLikedAsync(Guid profileId, Guid reviewId);

        // ────────────────────── Comments ──────────────────────
        Task AddCommentAsync(Guid profileId, Guid reviewId, CreateCommentRequest request);
        Task UpdateCommentAsync(Guid commentId, string newContent);
        Task DeleteCommentAsync(Guid commentId);
        Task<List<Comment>> GetTopLevelCommentsAsync(Guid reviewId);
        Task<List<Comment>> GetRepliesAsync(Guid parentCommentId);
    }
}