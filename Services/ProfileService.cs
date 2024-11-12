using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using poplensUserProfileApi.Contracts;
using poplensUserProfileApi.Data;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Models.Dtos;

namespace poplensUserProfileApi.Services {
    public class ProfileService : IProfileService {
        private readonly UserProfileDbContext _context;

        public ProfileService(UserProfileDbContext context) {
            _context = context;
        }

        public async Task<Profile> GetProfileAsync(Guid profileId) {
            return await _context.Profiles
                .Include(p => p.Followers)
                .Include(p => p.Following)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == profileId);
        }

        public async Task AddFollowerAsync(Guid followerId, Guid followingId) {
            // Check if the follow relationship already exists
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            // If the follow relation already exists, return a conflict response
            if (existingFollow != null) {
                throw new Exception("This follow relationship already exists.");
            }

            var follow = new Follow { FollowerId = followerId, FollowingId = followingId };
            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();
        }

        // Service Method
        public async Task<bool> UnfollowAsync(Guid followerId, Guid followingId) {
            // Find the follow relationship between the profiles
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null) {
                return false; // Follow relationship not found
            }

            // Remove the follow relationship from the DbContext
            _context.Follows.Remove(follow);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true; // Unfollow successful
        }


        public async Task AddReviewAsync(Guid profileId, CreateReviewRequest request) {
            // Validate review data (optional)
            if (request == null || string.IsNullOrEmpty(request.Content) || request.Rating <= 0) {
                throw new Exception("Invalid review data.");
            }
            var existingReview = await _context.Reviews.FirstOrDefaultAsync(r => r.ProfileId == profileId && r.MediaId == request.MediaId);
            if (existingReview != null) {
                // If a review exists, update it with the new values
                existingReview.Content = request.Content;
                existingReview.Rating = request.Rating;

                // Optionally, handle any other properties that you might want to update

                // Save the changes to the database
                await _context.SaveChangesAsync();
                return;
            }
            // Create a new Review entity
            var review = new Review {
                ProfileId = profileId,
                MediaId = request.MediaId,
                Content = request.Content,
                Rating = request.Rating
            };

            // Add the review to the database
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Optionally, you can update the media or profile's review list here
            // For example, if you want to add the review to the Profile's Reviews list
            // var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            // profile.Reviews.Add(review);

        }

        // Service Method
        public async Task<bool> DeleteReviewAsync(Guid profileId, string mediaId) {
            // Find the review for the specific profile and mediaId
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ProfileId == profileId && r.MediaId == mediaId);

            if (review == null) {
                return false; // Review not found
            }

            // Remove the review from the DbContext
            _context.Reviews.Remove(review);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true; // Review successfully deleted
        }


    }
}
