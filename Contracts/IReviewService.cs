using poplensUserProfileApi.Models;
using poplensUserProfileApi.Models.Dtos;

namespace poplensUserProfileApi.Contracts {
    public interface IReviewService {
        Task AddReviewAsync(Guid profileId, CreateReviewRequest request);
        Task<bool> DeleteReviewAsync(Guid profileId, string mediaId);
        Task<List<Review>> GetReviewsByProfileIdAsync(Guid profileId, int page = 1, int pageSize = 10);
    }
}