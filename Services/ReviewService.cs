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
using Pgvector.EntityFrameworkCore;
using Pgvector;
using poplensUserProfileApi.Models.Feed;

namespace poplensUserProfileApi.Services
{
    public class ReviewService : IReviewService {
        private readonly UserProfileDbContext _context;
        private readonly IUserAuthenticationApiProxyService _userAuthenticationApiProxyService;
        private readonly IMediaApiProxyService _mediaApiProxyService;
        private readonly IEmbeddingProxyService _embeddingProxyService;

        public ReviewService(UserProfileDbContext context, IUserAuthenticationApiProxyService userAuthenticationApiProxyService, IMediaApiProxyService mediaApiProxyService, IEmbeddingProxyService embeddingProxyService) {
            _context = context;
            _userAuthenticationApiProxyService = userAuthenticationApiProxyService;
            _mediaApiProxyService = mediaApiProxyService;
            _embeddingProxyService = embeddingProxyService;
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
                .Where(r => r.Id == reviewId)
                .Select(r => new Review {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating,
                    ProfileId = r.ProfileId,
                    MediaId = r.MediaId,
                    CreatedDate = r.CreatedDate,
                    LastUpdatedDate = r.LastUpdatedDate
                })
                .FirstOrDefaultAsync();
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
            var review = await GetReviewByIdAsync(reviewId);

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

            var media = await _mediaApiProxyService.GetMediaByIdAsync(new Guid(request.MediaId), token);
            var embeddingInputText = "Type: " + media.Type + "; " + "Title: " + media.Title + "; " + "Genre: " + media.Genre + "; " + "Review: " + request.Content + ";";
            var embedding = await _embeddingProxyService.GetEmbeddingAsync(embeddingInputText);

            if (existingReview != null) {
                existingReview.Content = request.Content;
                existingReview.Rating = request.Rating;
                existingReview.LastUpdatedDate = DateTime.UtcNow;
                existingReview.Embedding = embedding;
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
                LastUpdatedDate = DateTime.UtcNow,
                Embedding = embedding
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateMissingReviewEmbeddingsAsync(string token) {
            // Find reviews where Embedding is null
            var reviewsWithoutEmbedding = await _context.Reviews
                .Where(r => r.Embedding == null)
                .ToListAsync();

            int updatedCount = 0;

            foreach (var review in reviewsWithoutEmbedding) {
                // Get the related media for context
                var media = await _mediaApiProxyService.GetMediaByIdAsync(new Guid(review.MediaId), token);
                if (media == null)
                    continue;

                // Compose the embedding input text as in AddReviewAsync
                var embeddingInputText = $"Type: {media.Type}; Title: {media.Title}; Genre: {media.Genre}; Review: {review.Content};";
                var embedding = await _embeddingProxyService.GetEmbeddingAsync(embeddingInputText);

                if (embedding != null) {
                    review.Embedding = embedding;
                    updatedCount++;
                }
            }

            if (updatedCount > 0) {
                await _context.SaveChangesAsync();
            }

            return updatedCount;
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
            var query = _context.Reviews
                .Where(r => r.ProfileId == profileId)
                .OrderByDescending(r => r.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var reviews = await GetReviewsWithoutEmbeddingAsync(query);
            return reviews;
        }

        public async Task<List<Review>> GetReviewsByProfileIdsAsync(List<Guid> profileIds, int page = 1, int pageSize = 10) {
            if (profileIds == null || !profileIds.Any()) {
                return new List<Review>();
            }

            var query = _context.Reviews
                .Where(r => profileIds.Contains(r.ProfileId))
                .OrderByDescending(r => r.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var reviews = await GetReviewsWithoutEmbeddingAsync(query);
            return reviews;
        }

        public async Task<MediaMainPageReviewInfo> GetMediaMainPageReviewInfo(string mediaId, string token) {
            // Get all reviews for the specified mediaId
            var query = _context.Reviews
                .Where(r => r.MediaId == mediaId);
            var reviews = await GetReviewsWithoutEmbeddingAsync(query);

            // Extract profile IDs for username lookup
            var profileIds = reviews.Select(r => r.ProfileId).ToList();

            // Get usernames using the helper method
            var usernameMap = await GetUsernamesForProfileIdsAsync(profileIds, token);

            // Map reviews to ReviewWithUsername
            var reviewsWithUsernames = reviews.Select(r => new ReviewWithUsername {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                ProfileId = r.ProfileId,
                MediaId = r.MediaId,
                CreatedDate = r.CreatedDate,
                LastUpdatedDate = r.LastUpdatedDate,
                Username = usernameMap.TryGetValue(r.ProfileId, out var username) ? username : "Unknown"
            }).ToList();

            // Calculate rating chart info
            var ratingChartInfo = reviewsWithUsernames
                .GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => (float)g.Count());

            // Get popular reviews (top 5 reviews with the highest ratings)
            var popularReviews = reviewsWithUsernames
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.CreatedDate)
                .Take(5)
                .ToList();

            // Get recent reviews (top 5 most recent reviews)
            var recentReviews = reviewsWithUsernames
                .OrderByDescending(r => r.CreatedDate)
                .Take(5)
                .ToList();

            // Create and return the MediaMainPageReviewInfo object
            return new MediaMainPageReviewInfo {
                RatingChartInfo = ratingChartInfo,
                PopularReviews = popularReviews,
                RecentReviews = recentReviews
            };
        }

        public async Task<PageResult<ReviewWithUsername>> GetMediaReviews(string mediaId, int page, int pageSize, string sortOption, string token) {
            var query = _context.Reviews
                .Where(r => r.MediaId == mediaId);

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
            query = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            var reviews = await GetReviewsWithoutEmbeddingAsync(query);
            // Extract profile IDs for username lookup
            var profileIds = reviews.Select(r => r.ProfileId).ToList();

            // Get usernames using the helper method
            var usernameMap = await GetUsernamesForProfileIdsAsync(profileIds, token);

            // Map reviews to ReviewWithUsername
            var reviewsWithUsernames = reviews.Select(r => new ReviewWithUsername {
                Id = r.Id,
                Content = r.Content,
                Rating = r.Rating,
                ProfileId = r.ProfileId,
                MediaId = r.MediaId,
                CreatedDate = r.CreatedDate,
                LastUpdatedDate = r.LastUpdatedDate,
                Username = usernameMap.TryGetValue(r.ProfileId, out var username) ? username : "Unknown"
            }).ToList();

            return new PageResult<ReviewWithUsername> {
                Result = reviewsWithUsernames,
                TotalCount = totalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Gets reviews with embeddings for a profile
        /// </summary>
        public async Task<List<Review>> GetReviewsWithEmbeddingsAsync(Guid profileId) {
            var reviews = await _context.Reviews
                .Where(r => r.ProfileId == profileId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();

            return reviews;
        }

        /// <summary>
        /// Gets reviews that a profile has liked, including their embeddings
        /// </summary>
        public async Task<List<Review>> GetLikedReviewsWithEmbeddingsAsync(Guid profileId) {
            var likedReviewIds = await _context.Likes
                .Where(l => l.ProfileId == profileId)
                .Select(l => l.ReviewId)
                .ToListAsync();

            var likedReviews = await _context.Reviews
                .Where(r => likedReviewIds.Contains(r.Id))
                .ToListAsync();

            return likedReviews;
        }

        /// <summary>
        /// Gets reviews that a profile has commented on, including their embeddings
        /// </summary>
        public async Task<List<Review>> GetCommentedReviewsWithEmbeddingsAsync(Guid profileId) {
            var commentedReviewIds = await _context.Comments
                .Where(c => c.ProfileId == profileId && c.ParentCommentId == null)
                .Select(c => c.ReviewId)
                .Distinct()
                .ToListAsync();

            var commentedReviews = await _context.Reviews
                .Where(r => commentedReviewIds.Contains(r.Id))
                .ToListAsync();

            return commentedReviews;
        }

        /// <summary>
        /// Gets all user interactions (reviews, likes, comments) with embeddings in one request
        /// </summary>
        public async Task<UserInteractionsResponse> GetUserInteractionsWithEmbeddingsAsync(Guid profileId) {
            // Get the user's own reviews with embeddings
            var userReviews = await GetReviewsWithEmbeddingsAsync(profileId);

            // Get reviews the user has liked with embeddings
            var likedReviews = await GetLikedReviewsWithEmbeddingsAsync(profileId);

            // Get reviews the user has commented on with embeddings
            var commentedReviews = await GetCommentedReviewsWithEmbeddingsAsync(profileId);

            return new UserInteractionsResponse {
                OwnReviews = userReviews,
                LikedReviews = likedReviews,
                CommentedReviews = commentedReviews
            };
        }

        /// <summary>
        /// Gets reviews with embeddings similar to the provided embedding, excluding any already displayed to the user
        /// </summary>
        /// <param name="embedding">The embedding vector to match against</param>
        /// <param name="count">Number of reviews to return</param>
        /// <param name="excludedReviewIds">Optional list of review IDs to exclude from results</param>
        /// <returns>List of similar reviews with their embeddings</returns>
        public async Task<List<Review>> GetSimilarReviewsAsync(Vector embedding, int count, List<Guid>? excludedReviewIds = null, Guid? requestingProfileId = null) {
            // Base query with embedding similarity using cosine distance
            var query = _context.Reviews.Where(r => r.Embedding != null);

            // Exclude reviews by the requesting user
            if (requestingProfileId.HasValue) {
                query = query.Where(r => r.ProfileId != requestingProfileId.Value);
            }

            // If we have excluded review IDs, filter them out
            if (excludedReviewIds != null && excludedReviewIds.Any()) {
                query = query.Where(r => !excludedReviewIds.Contains(r.Id));
            }

            // Order by similarity and take the requested number
            var reviews = await query
                .OrderBy(r => r.Embedding.CosineDistance(embedding))
                .Take(count)
                .ToListAsync();

            return reviews;
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
        /// Retrieves all top-level comments for a review, including one level of replies with usernames.
        /// </summary>
        public async Task<List<CommentDetail>> GetTopLevelCommentsAsync(Guid reviewId, string token) {
            // Fetch comments with replies
            List<Comment> comments = await _context.Comments
                .Where(c => c.ReviewId == reviewId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .ToListAsync();

            // Get all profile IDs from both top-level comments and replies
            var profileIds = comments.Select(c => c.ProfileId).ToList();

            // Add profile IDs from replies
            foreach (var comment in comments) {
                if (comment.Replies != null && comment.Replies.Any()) {
                    profileIds.AddRange(comment.Replies.Select(r => r.ProfileId));
                }
            }

            // Make profile IDs distinct
            profileIds = profileIds.Distinct().ToList();

            // Get usernames in a single method call
            var usernameMap = await GetUsernamesForProfileIdsAsync(profileIds, token);

            // Map comments to comment details with usernames
            List<CommentDetail> commentDetails = new List<CommentDetail>();
            foreach (var comment in comments) {
                var detail = new CommentDetail();
                MapCommentToCommentDetail(comment, detail);
                detail.Username = usernameMap.TryGetValue(comment.ProfileId, out var username) ? username : "Unknown";

                // Set the reply count
                detail.ReplyCount = comment.Replies?.Count ?? 0;

                // Process replies
                if (detail.Replies != null) {
                    var replyDetails = new List<CommentDetail>();
                    foreach (var reply in detail.Replies) {
                        var replyDetail = new CommentDetail();
                        // Copy properties
                        MapCommentToCommentDetail(reply, replyDetail);
                        // Add username
                        replyDetail.Username = usernameMap.TryGetValue(reply.ProfileId, out var replyUsername) ? replyUsername : "Unknown";

                        // Set nested reply count
                        replyDetail.ReplyCount = await GetReplyCountAsync(reply.Id);

                        replyDetails.Add(replyDetail);
                    }

                    // Replace the original replies with the detailed ones
                    detail.DetailedReplies = replyDetails;
                }

                commentDetails.Add(detail);
            }

            return commentDetails;
        }

        /// <summary>
        /// Retrieves direct replies to a specific comment, including one more level of nested replies.
        /// </summary>
        public async Task<List<CommentDetail>> GetRepliesAsync(Guid parentCommentId, string token) {
            // Fetch replies with their replies
            List<Comment> comments = await _context.Comments
                .Where(c => c.ParentCommentId == parentCommentId)
                .Include(c => c.Replies)
                .ToListAsync();

            // Get all profile IDs from both replies and nested replies
            var profileIds = comments.Select(c => c.ProfileId).ToList();

            // Add profile IDs from nested replies
            foreach (var comment in comments) {
                if (comment.Replies != null && comment.Replies.Any()) {
                    profileIds.AddRange(comment.Replies.Select(r => r.ProfileId));
                }
            }

            // Make profile IDs distinct
            profileIds = profileIds.Distinct().ToList();

            // Get usernames
            var usernameMap = await GetUsernamesForProfileIdsAsync(profileIds, token);

            // Map to comment details
            List<CommentDetail> commentDetails = new List<CommentDetail>();
            foreach (var comment in comments) {
                var detail = new CommentDetail();
                MapCommentToCommentDetail(comment, detail);
                detail.Username = usernameMap.TryGetValue(comment.ProfileId, out var username) ? username : "Unknown";

                // Set the reply count
                detail.ReplyCount = comment.Replies?.Count ?? 0;

                // Process nested replies
                if (detail.Replies != null) {
                    var replyDetails = new List<CommentDetail>();
                    foreach (var reply in detail.Replies) {
                        var replyDetail = new CommentDetail();
                        // Copy properties
                        MapCommentToCommentDetail(reply, replyDetail);
                        // Add username
                        replyDetail.Username = usernameMap.TryGetValue(reply.ProfileId, out var replyUsername) ? replyUsername : "Unknown";

                        // Set nested reply count
                        replyDetail.ReplyCount = await GetReplyCountAsync(reply.Id);

                        replyDetails.Add(replyDetail);
                    }

                    // Replace the original replies with the detailed ones
                    detail.DetailedReplies = replyDetails;
                }

                commentDetails.Add(detail);
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

        /// <summary>
        /// Gets the total number of replies for a comment.
        /// </summary>
        /// <param name="parentCommentId">The unique identifier of the parent comment</param>
        /// <returns>The count of replies associated with the comment</returns>
        public async Task<int> GetReplyCountAsync(Guid parentCommentId) {
            return await _context.Comments
                .CountAsync(c => c.ParentCommentId == parentCommentId);
        }

        // ────────────────────────────────────────────────────────────
        // Helpers
        // ────────────────────────────────────────────────────────────

        /// <summary>
        /// Helper method to obtain usernames for a collection of comments.
        /// </summary>
        private async Task<Dictionary<Guid, string>> GetUsernamesForProfileIdsAsync(List<Guid> profileIds, string token) {
            // Create a mapping dictionary to return
            var profileIdToUsernameMap = new Dictionary<Guid, string>();

            if (profileIds == null || !profileIds.Any())
                return profileIdToUsernameMap;

            // Get the userId for each profileId
            var profileIdToUserIdMap = await _context.Profiles
                .Where(p => profileIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.UserId);

            // Get all userIds to fetch usernames in a single call
            var userIds = profileIdToUserIdMap.Values.ToList();

            // Get usernames for all userIds in one API call
            var userIdToUsernameMap = await _userAuthenticationApiProxyService.GetUsernamesByUserIdsAsync(userIds, token);

            // Map each profileId directly to its username
            foreach (var profileId in profileIds) {
                if (profileIdToUserIdMap.TryGetValue(profileId, out var userId) &&
                    userIdToUsernameMap.TryGetValue(new Guid(userId), out var username)) {
                    profileIdToUsernameMap[profileId] = username;
                } else {
                    profileIdToUsernameMap[profileId] = "Unknown";
                }
            }

            return profileIdToUsernameMap;
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
            commentDetail.ReplyCount = comment.Replies?.Count ?? 0;
        }

        private async Task<List<Review>> GetReviewsWithoutEmbeddingAsync(IQueryable<Review> query) {
            // Project only the fields you need, excluding Embedding
            return await query
                .Select(r => new Review {
                    Id = r.Id,
                    Content = r.Content,
                    Rating = r.Rating,
                    ProfileId = r.ProfileId,
                    MediaId = r.MediaId,
                    CreatedDate = r.CreatedDate,
                    LastUpdatedDate = r.LastUpdatedDate
                    // Embedding is not selected
                })
                .ToListAsync();
        }

    }

}
