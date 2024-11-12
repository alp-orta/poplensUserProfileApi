using Microsoft.AspNetCore.Mvc;
using poplensUserProfileApi.Contracts;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Models.Dtos;

namespace poplensUserProfileApi.Controllers {
    // ProfileController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService) {
            _profileService = profileService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(Guid id) {
            var profile = await _profileService.GetProfileAsync(id);
            if (profile == null) return NotFound();

            return Ok(profile);
        }

        [HttpPost("{id}/follow")]
        public async Task<IActionResult> FollowUser(Guid id, [FromBody] Guid followingId) {
            await _profileService.AddFollowerAsync(id, followingId);
            return Ok();
        }

        // Controller Method
        [HttpDelete("{followerId}/unfollow/{followingId}")]
        public async Task<IActionResult> Unfollow(Guid followerId, Guid followingId) {
            var result = await _profileService.UnfollowAsync(followerId, followingId);

            if (!result) {
                return NotFound("Follow relationship not found.");
            }

            return NoContent(); // Successfully unfollowed
        }


        [HttpPost("{id}/addReview")]
        public async Task<ActionResult> AddReview(Guid id, CreateReviewRequest request) {
            await _profileService.AddReviewAsync(id, request);
            return Ok();
        }

        [HttpDelete("{profileId}/deleteReview/{mediaId}")]
        public async Task<IActionResult> DeleteReview(Guid profileId, string mediaId) {
            var result = await _profileService.DeleteReviewAsync(profileId, mediaId);

            if (!result) {
                return NotFound("Review not found.");
            }

            return NoContent(); // Successfully deleted
        }

    }

}
