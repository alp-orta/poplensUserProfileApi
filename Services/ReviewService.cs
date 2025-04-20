using poplensUserProfileApi.Data;
using poplensUserProfileApi.Models.Dtos;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Contracts;
using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Models.Common;
using System.Net.Http;

namespace poplensUserProfileApi.Services
{
    public class ReviewService : IReviewService {
        private readonly UserProfileDbContext _context;
        private readonly IUserAuthenticationApiProxyService _userAuthenticationApiProxyService;

        public ReviewService(UserProfileDbContext context, IUserAuthenticationApiProxyService userAuthenticationApiProxyService) {
            _context = context;
            _userAuthenticationApiProxyService = userAuthenticationApiProxyService;
        }

        public async Task AddReviewAsync(Guid profileId, CreateReviewRequest request) {
            if (request == null || string.IsNullOrEmpty(request.Content) || request.Rating <= 0) {
                throw new Exception("Invalid review data.");
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProfileId == profileId && r.MediaId == request.MediaId);

            if (existingReview != null) {
                existingReview.Content = request.Content;
                existingReview.Rating = request.Rating;
                existingReview.LastUpdatedDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return;
            }

            var review = new Review {
                ProfileId = profileId,
                MediaId = request.MediaId,
                Content = request.Content,
                Rating = request.Rating,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteReviewAsync(Guid profileId, string mediaId) {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProfileId == profileId && r.MediaId == mediaId);

            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Review>> GetReviewsByProfileIdAsync(Guid profileId, int page = 1, int pageSize = 10) {
            var reviews = await _context.Reviews
                .Where(r => r.ProfileId == profileId)
                .OrderByDescending(r => r.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return reviews;
        }

        public async Task<List<Review>> GetReviewsByProfileIdsAsync(List<Guid> profileIds, int page = 1, int pageSize = 10) {
            if (profileIds == null || !profileIds.Any()) {
                return new List<Review>();
            }

            var reviews = await _context.Reviews
                .Where(r => profileIds.Contains(r.ProfileId))
                .OrderByDescending(r => r.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return reviews;
        }

        public async Task<MediaMainPageReviewInfo> GetMediaMainPageReviewInfo(string mediaId, string token) {
            // Get all reviews for the specified mediaId
            var reviews = await _context.Reviews
                .Where(r => r.MediaId == mediaId)
                .ToListAsync();

            // Extract profile IDs from reviews
            var profileIds = reviews.Select(r => r.ProfileId.ToString()).ToList();
            var profilIdToUserIdMap = await _context.Profiles
                .Where(p => profileIds.Contains(p.Id.ToString()))
                .ToDictionaryAsync(p => p.Id, p => p.UserId);
            var userIds = profilIdToUserIdMap.Values.ToList();

            // Get usernames by profile IDs

            var usernames = await _userAuthenticationApiProxyService.GetUsernamesByIdsAsync(userIds, token);

            // Map reviews to ReviewWithUsername
            var reviewsWithUsernames = reviews.Select(r => new ReviewWithUsername {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                ProfileId = r.ProfileId,
                MediaId = r.MediaId,
                CreatedDate = r.CreatedDate,
                LastUpdatedDate = r.LastUpdatedDate,
                Username = usernames.ContainsKey(new Guid(profilIdToUserIdMap[r.ProfileId])) ? usernames[new Guid(profilIdToUserIdMap[r.ProfileId])] : "Unknown"
            }).ToList();

            // Calculate rating chart info
            var ratingChartInfo = reviewsWithUsernames
                .GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => (float)g.Count());

            // Get popular reviews (e.g., top 5 reviews with the highest ratings)
            var popularReviews = reviewsWithUsernames
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.CreatedDate)
                .Take(5)
                .ToList();

            // Get recent reviews (e.g., top 5 most recent reviews)
            var recentReviews = reviewsWithUsernames
                .OrderByDescending(r => r.CreatedDate)
                .Take(5)
                .ToList();

            // Create and return the MediaMainPageReviewInfo object
            var mediaMainPageReviewInfo = new MediaMainPageReviewInfo {
                RatingChartInfo = ratingChartInfo,
                PopularReviews = popularReviews,
                RecentReviews = recentReviews
            };

            return mediaMainPageReviewInfo;
        }

        public async Task<PageResult<ReviewWithUsername>> GetMediaReviews(string mediaId, int page, int pageSize, string sortOption, string token) {
            var query = _context.Reviews.Where(r => r.MediaId == mediaId);

            switch (sortOption.ToLower()) {
                case "highestrated":
                    query = query.OrderByDescending(r => r.Rating);
                    break;
                case "lowestrated":
                    query = query.OrderBy(r => r.Rating);
                    break;
                case "mostrecent":
                    query = query.OrderByDescending(r => r.CreatedDate);
                    break;
                default:
                    throw new ArgumentException("Invalid sort option");
            }
            var totalItems = await query.CountAsync();
            var reviews = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var profileIds = reviews.Select(r => r.ProfileId.ToString()).ToList();

            // Get usernames by profile IDs
            var usernames = await _userAuthenticationApiProxyService.GetUsernamesByIdsAsync(profileIds, token);

            // Map reviews to ReviewWithUsername
            var reviewsWithUsernames = reviews.Select(r => new ReviewWithUsername {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                ProfileId = r.ProfileId,
                MediaId = r.MediaId,
                CreatedDate = r.CreatedDate,
                LastUpdatedDate = r.LastUpdatedDate,
                Username = usernames.ContainsKey(r.ProfileId) ? usernames[r.ProfileId] : "Unknown"
            }).ToList();

            var pageResult = new PageResult<ReviewWithUsername> {
                Result = reviewsWithUsernames,
                TotalCount = totalItems,
                Page = page,
                PageSize = pageSize
            };

            return pageResult;
        }
    }

}
