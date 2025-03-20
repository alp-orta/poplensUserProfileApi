using poplensUserProfileApi.Models;

namespace poplensUserProfileApi.Contracts {
    public interface IProfileService {
        Task<Profile> GetProfileAsync(Guid profileId, string token);
        Task<Profile> CreateProfileAsync(string userId);
        Task AddFollowerAsync(Guid followerId, Guid followingId);
        Task<bool> UnfollowAsync(Guid followerId, Guid followingId);
        Task<List<FollowedProfile>> GetFollowingListAsync(Guid profileId, string token);
        Task<Guid> GetProfileIdWithUserIdAsync(string userId, string token);
    }
}
