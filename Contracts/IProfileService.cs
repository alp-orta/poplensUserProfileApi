using Microsoft.AspNetCore.Mvc;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Models.Dtos;

namespace poplensUserProfileApi.Contracts {
    public interface IProfileService {
        Task<Profile> GetProfileAsync(Guid profileId);
        Task AddFollowerAsync(Guid followerId, Guid followingId);
        Task<bool> UnfollowAsync(Guid followerId, Guid followingId);
        Task AddReviewAsync(Guid profileId, CreateReviewRequest request);
        Task<bool> DeleteReviewAsync(Guid profileId, string mediaId);
    }
}
