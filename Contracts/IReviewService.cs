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
    }
}