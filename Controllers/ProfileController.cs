using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using poplensUserProfileApi.Contracts;

namespace poplensUserProfileApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService) {
            _profileService = profileService;
        }

        [HttpGet("GetProfileIdWithUserId/{userId}")]
        public async Task<IActionResult> GetProfileIdWithUserId(string userId) {
            var profileId = await _profileService.GetProfileIdWithUserIdAsync(userId, GetTokenFromRequest());
            if (profileId == Guid.Empty) return NotFound();

            return Ok(profileId);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetProfile/{profileId}")]
        public async Task<IActionResult> GetProfile(Guid profileId) {
            var token = GetTokenFromRequest();
            var profile = await _profileService.GetProfileAsync(profileId, token);
            if (profile == null) return NotFound();

            return Ok(profile);
        }

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreateProfile(string userId) {
            var profile = await _profileService.CreateProfileAsync(userId);
            return CreatedAtAction(nameof(GetProfile), new { profileId = profile.Id }, profile);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{followerId}/follow/{followingId}")]
        public async Task<IActionResult> Follow(Guid followerId, Guid followingId) {
            var token = GetTokenFromRequest();
            await _profileService.AddFollowerAsync(followerId, followingId);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{followerId}/unfollow/{followingId}")]
        public async Task<IActionResult> Unfollow(Guid followerId, Guid followingId) {
            var result = await _profileService.UnfollowAsync(followerId, followingId);
            if (!result) return NotFound();

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{profileId}/followinglist")]
        public async Task<IActionResult> GetFollowingList(Guid profileId) {
            var token = GetTokenFromRequest();
            var followingIds = await _profileService.GetFollowingListAsync(profileId, token);
            if (followingIds == null || !followingIds.Any()) {
                return NotFound("No profiles being followed.");
            }

            return Ok(followingIds);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetProfilesWithUsernames")]
        public async Task<IActionResult> GetProfilesWithUsernames([FromQuery] List<Guid> profileIds) {
            if (profileIds == null || !profileIds.Any()) {
                return BadRequest("Profile IDs are required");
            }

            var token = GetTokenFromRequest();
            var profiles = await _profileService.GetProfilesWithUsernamesAsync(profileIds, token);

            if (profiles == null || !profiles.Any()) {
                return NotFound("No profiles found with the specified IDs");
            }

            return Ok(profiles);
        }

        private string GetTokenFromRequest() {
            return Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }
    }
}
