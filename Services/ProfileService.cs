using Microsoft.EntityFrameworkCore;
using poplensMediaApi.Models;
using poplensUserProfileApi.Contracts;
using poplensUserProfileApi.Data;
using poplensUserProfileApi.Models;

namespace poplensUserProfileApi.Services {
    public class ProfileService : IProfileService {
        private readonly UserProfileDbContext _context;
        private readonly IMediaApiProxyService _mediaApiProxyService;
        private readonly IUserAuthenticationApiProxyService _userAuthenticationApiProxyService;

        public ProfileService(UserProfileDbContext context,
                              IMediaApiProxyService mediaApiProxyService,
                              IUserAuthenticationApiProxyService userAuthenticationApiProxyService) {
            _context = context;
            _mediaApiProxyService = mediaApiProxyService;
            _userAuthenticationApiProxyService = userAuthenticationApiProxyService;
        }

        public async Task<Profile> GetProfileAsync(Guid profileId, string token) {
            var profile = await _context.Profiles
                .Include(p => p.Followers)
                .Include(p => p.Following)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == profileId);

            if (profile != null) {
                profile.DetailedReviews = await GetReviewDetailsAsync(profile.Reviews, token);
            }

            return profile;
        }

        private async Task<List<ReviewDetail>> GetReviewDetailsAsync(List<Review> reviews, string token) {
            var reviewDetails = new List<ReviewDetail>();

            foreach (var review in reviews) {
                var media = await _mediaApiProxyService.GetMediaByIdAsync(Guid.Parse(review.MediaId), token);
                if (media != null) {
                    var reviewDetail = new ReviewDetail {
                        Id = review.Id,
                        Content = review.Content,
                        Rating = review.Rating,
                        ProfileId = review.ProfileId,
                        MediaId = review.MediaId,
                        CreatedDate = review.CreatedDate,
                        LastUpdatedDate = review.LastUpdatedDate,
                        // Fetch additional fields from another API
                        MediaTitle = media.Title,
                        MediaType = media.Type,
                        MediaCachedImagePath = media.CachedImagePath,
                        MediaCreator = GetCreator(media)
                    };

                    reviewDetails.Add(reviewDetail);
                }
            }

            return reviewDetails.OrderByDescending(r => r.CreatedDate).ToList();
        }

        private string GetCreator(Media media) {
            return media.Type switch {
                "film" => media.Director,
                "book" => media.Writer,
                "game" => media.Publisher,
                _ => "Unknown"
            };
        }

        public async Task<Guid> GetProfileIdWithUserIdAsync(string userId, string token) {
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
            return profile?.Id ?? Guid.Empty;
        }

        public async Task<Profile> CreateProfileAsync(string userId) {
            if (await _context.Profiles.AnyAsync(p => p.UserId == userId)) {
                throw new InvalidOperationException("A profile for this user already exists.");
            }

            var profile = new Profile {
                Id = Guid.NewGuid(),
                UserId = userId
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();

            return profile;
        }

        public async Task AddFollowerAsync(Guid followerId, Guid followingId) {
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (existingFollow != null) {
                throw new Exception("This follow relationship already exists.");
            }

            var follow = new Follow {
                FollowerId = followerId,
                FollowingId = followingId,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };
            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UnfollowAsync(Guid followerId, Guid followingId) {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null) return false;

            _context.Follows.Remove(follow);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<FollowedProfile>> GetFollowingListAsync(Guid profileId, string token) {
            // Get the list of follow relationships where the given profile is following other profiles
            var followingProfileIds = await _context.Follows
                .Where(f => f.FollowerId == profileId)
                .Select(f => f.FollowingId)
                .ToListAsync();

            // Get the user IDs associated with the following profile IDs
            var followingUserIds = await _context.Profiles
                .Where(p => followingProfileIds.Contains(p.Id))
                .Select(p => p.UserId)
                .ToListAsync();

            // Get usernames from UserAuth API using the provided token
            Dictionary<Guid, string> usernames = await _userAuthenticationApiProxyService.GetUsernamesByUserIdsAsync(followingUserIds, token);

            // Map the results to FollowedProfile objects
            var followedProfiles = followingProfileIds.Select(id => {
                var userId = _context.Profiles.FirstOrDefault(p => p.Id == id)?.UserId;
                return new FollowedProfile {
                    ProfileId = id,
                    Username = userId != null && usernames.ContainsKey(Guid.Parse(userId)) ? usernames[Guid.Parse(userId)] : "Unknown"
                };
            }).ToList();

            return followedProfiles;
        }
    }
}
