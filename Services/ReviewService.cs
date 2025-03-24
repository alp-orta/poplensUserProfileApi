using poplensUserProfileApi.Data;
using poplensUserProfileApi.Models.Dtos;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Contracts;
using Microsoft.EntityFrameworkCore;

namespace poplensUserProfileApi.Services
{
    public class ReviewService : IReviewService {
        private readonly UserProfileDbContext _context;

        public ReviewService(UserProfileDbContext context) {
            _context = context;
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
    }

}
