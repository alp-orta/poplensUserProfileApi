using poplensUserProfileApi.Data;
using poplensUserProfileApi.Models.Dtos;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Contracts;
using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Models.Common;
using System.Net.Http;
using poplensMediaApi.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace poplensUserProfileApi.Services
{
    public class ReviewService : IReviewService {
        private readonly UserProfileDbContext _context;
        private readonly IUserAuthenticationApiProxyService _userAuthenticationApiProxyService;
        private readonly IMediaApiProxyService _mediaApiProxyService;

        public ReviewService(UserProfileDbContext context, IUserAuthenticationApiProxyService userAuthenticationApiProxyService, IMediaApiProxyService mediaApiProxyService) {
            _context = context;
            _userAuthenticationApiProxyService = userAuthenticationApiProxyService;
            _mediaApiProxyService = mediaApiProxyService;
        }
        
        // ────────────────────────────────────────────────────────────
        // Reviews
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets a review by its unique identifier.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review</param>
        /// <returns>The review if found, otherwise null</returns>
        public async Task<Review> GetReviewByIdAsync(Guid reviewId) {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            return review;
        }

        /// <summary>
        /// Gets detailed information about a review, including username and media information.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review</param>
        /// <param name="token">The authorization token for API calls</param>
        /// <returns>A ReviewDetail object with extended information</returns>
        public async Task<ReviewDetail> GetReviewDetailAsync(Guid reviewId, string token) {
            // Get the base review
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return null;

            // Get the profile and user info
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Id == review.ProfileId);

            if (profile == null)
                return null;

            // Get username from auth service
            var usernames = await _userAuthenticationApiProxyService.GetUsernamesByUserIdsAsync(
                new List<string> { profile.UserId },
                token);

            // Get media details
            var media = await _mediaApiProxyService.GetMediaByIdAsync(
                Guid.Parse(review.MediaId),
                token);

            if (media == null)
                return null;

            // Create and populate the ReviewDetail object
            var reviewDetail = new ReviewDetail {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                ProfileId = review.ProfileId,
                MediaId = review.MediaId,
                CreatedDate = review.CreatedDate,
                LastUpdatedDate = review.LastUpdatedDate,
                // Add media information
                MediaTitle = media.Title,
                MediaType = media.Type,
                MediaCachedImagePath = media.CachedImagePath,
                MediaCreator = GetCreator(media)
            };

            // Add username if available
            if (usernames.ContainsKey(Guid.Parse(profile.UserId))) {
                reviewDetail.Username = usernames[Guid.Parse(profile.UserId)];
            } else {
                reviewDetail.Username = "Unknown";
            }

            return reviewDetail;
        }

        private string GetCreator(Media media) {
            return media.Type switch {
                "film" => media.Director,
                "book" => media.Writer,
                "game" => media.Publisher,
                _ => "Unknown"
            };
        }


        public async Task AddReviewAsync(Guid profileId, CreateReviewRequest request, string token) {
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
            if (!await _mediaApiProxyService.IncrementTotalReviewCountAsync(new Guid(request.MediaId), token)) {
                throw new Exception("Error while incrementing media total review Count");
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

            var usernames = await _userAuthenticationApiProxyService.GetUsernamesByUserIdsAsync(userIds, token);

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
            var usernames = await _userAuthenticationApiProxyService.GetUsernamesByUserIdsAsync(profileIds, token);

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

        // ────────────────────────────────────────────────────────────
        // Likes
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a like for the given review by the given profile.
        /// </summary>
        public async Task AddLikeAsync(Guid profileId, Guid reviewId) {
            if (await _context.Likes.AnyAsync(l => l.ProfileId == profileId && l.ReviewId == reviewId))
                throw new Exception("You have already liked this review.");

            var like = new Like {
                Id = Guid.NewGuid(),
                ProfileId = profileId,
                ReviewId = reviewId,
                CreatedDate = DateTime.UtcNow
            };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a like for the given review by the given profile.
        /// </summary>
        public async Task RemoveLikeAsync(Guid profileId, Guid reviewId) {
            var like = await _context.Likes
                .FirstOrDefaultAsync(l => l.ProfileId == profileId && l.ReviewId == reviewId);

            if (like == null)
                throw new Exception("Like not found.");

            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the total number of likes for a review.
        /// </summary>
        public async Task<int> GetLikeCountAsync(Guid reviewId) {
            return await _context.Likes.CountAsync(l => l.ReviewId == reviewId);
        }

        /// <summary>
        /// Checks if a profile has liked a given review.
        /// </summary>
        public async Task<bool> HasUserLikedAsync(Guid profileId, Guid reviewId) {
            return await _context.Likes.AnyAsync(l => l.ProfileId == profileId && l.ReviewId == reviewId);
        }

        // ────────────────────────────────────────────────────────────
        // Comments (threaded)
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Adds a new comment (or reply) to a review.
        /// </summary>
        public async Task AddCommentAsync(Guid profileId, Guid reviewId, CreateCommentRequest request) {
            if (string.IsNullOrWhiteSpace(request.Content))
                throw new Exception("Comment content cannot be empty.");

            // If this is a reply, verify the parent exists
            if (request.ParentCommentId.HasValue) {
                var parent = await _context.Comments
                    .FirstOrDefaultAsync(c => c.Id == request.ParentCommentId.Value && c.ReviewId == reviewId);
                if (parent == null)
                    throw new Exception("Parent comment not found or does not belong to this review.");
            }

            var comment = new Comment {
                Id = Guid.NewGuid(),
                ProfileId = profileId,
                ReviewId = reviewId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Updates the content of an existing comment.
        /// </summary>
        public async Task UpdateCommentAsync(Guid commentId, string newContent) {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                throw new Exception("Comment not found.");

            comment.Content = newContent;
            comment.LastUpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes a comment (note: replies will remain, with their ParentCommentId unchanged).
        /// </summary>
        public async Task DeleteCommentAsync(Guid commentId) {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                throw new Exception("Comment not found.");

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves all top-level comments for a review, including one level of replies.
        /// </summary>
        public async Task<List<CommentDetail>> GetTopLevelCommentsAsync(Guid reviewId, string token) {
            List<Comment> comments = await _context.Comments
                .Where(c => c.ReviewId == reviewId && c.ParentCommentId == null)
                .Include(c => c.Replies)         // first-level replies
                .ToListAsync();
            var profileIds = comments.Select(c => c.ProfileId).ToList();
            var profileIdToUserIdMap = await _context.Profiles
                .Where(p => profileIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.UserId);
            var userIds = profileIdToUserIdMap.Values.ToList();
            var usernames = await _userAuthenticationApiProxyService.GetUsernamesByUserIdsAsync(userIds, token);
            List<CommentDetail> commentDetails = new List<CommentDetail>();
            foreach (var comment in comments) {
                CommentDetail commentDetail = new CommentDetail();
                MapCommentToCommentDetail(comment, commentDetail);
                commentDetail.Username = usernames.ContainsKey(new Guid(profileIdToUserIdMap[commentDetail.ProfileId])) ? usernames[new Guid(profileIdToUserIdMap[commentDetail.ProfileId])] : "Unknown";
                commentDetails.Add(commentDetail);
            }
            return commentDetails;
        }


        /// <summary>
        /// Retrieves direct replies to a specific comment, including one more level of nested replies.
        /// </summary>
        public async Task<List<CommentDetail>> GetRepliesAsync(Guid parentCommentId, string token) {
            List<Comment> comments = await _context.Comments
                .Where(c => c.ParentCommentId == parentCommentId)
                .Include(c => c.Replies)         // next-level replies
                .ToListAsync();
            var profileIds = comments.Select(c => c.ProfileId).ToList();
            var profileIdToUserIdMap = await _context.Profiles
                .Where(p => profileIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.UserId);
            var userIds = profileIdToUserIdMap.Values.ToList();
            var usernames = await _userAuthenticationApiProxyService.GetUsernamesByUserIdsAsync(userIds, token);
            List<CommentDetail> commentDetails = new List<CommentDetail>();
            foreach (var comment in comments) {
                CommentDetail commentDetail = new CommentDetail();
                MapCommentToCommentDetail(comment, commentDetail);
                commentDetail.Username = usernames.ContainsKey(new Guid(profileIdToUserIdMap[commentDetail.ProfileId])) ? usernames[new Guid(profileIdToUserIdMap[commentDetail.ProfileId])] : "Unknown";
                commentDetails.Add(commentDetail);
            }
            return commentDetails;
        }

        /// <summary>
        /// Gets the total number of comments for a review.
        /// </summary>
        /// <param name="reviewId">The unique identifier of the review</param>
        /// <returns>The count of comments associated with the review</returns>
        public async Task<int> GetCommentCountAsync(Guid reviewId) {
            return await _context.Comments
                .CountAsync(c => c.ReviewId == reviewId);
        }

        // ────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────
        private async Task<List<string>> GetUserIIdsFromProfileIds(List<Guid> profileIds) {
            var profilIdToUserIdMap = await _context.Profiles
                .Where(p => profileIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.UserId);
            return profilIdToUserIdMap.Values.ToList();
        }

        private void MapCommentToCommentDetail(Comment comment, CommentDetail commentDetail) {
            commentDetail.Id = comment.Id;
            commentDetail.ReviewId = comment.ReviewId;
            commentDetail.ParentCommentId = comment.ParentCommentId;
            commentDetail.ProfileId = comment.ProfileId;
            commentDetail.Content = comment.Content;
            commentDetail.CreatedDate = comment.CreatedDate;
            commentDetail.LastUpdatedDate = comment.LastUpdatedDate;
            commentDetail.Replies = comment.Replies;
        }

    }

}
