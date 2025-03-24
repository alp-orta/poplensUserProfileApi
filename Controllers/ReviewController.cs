using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using poplensUserProfileApi.Contracts;
using poplensUserProfileApi.Models;
using poplensUserProfileApi.Models.Dtos;

namespace poplensUserProfileApi.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService) {
            _reviewService = reviewService;
        }

        [HttpPost("{profileId}/addReview")]
        public async Task<IActionResult> AddReview(Guid profileId, [FromBody] CreateReviewRequest request) {
            await _reviewService.AddReviewAsync(profileId, request);
            return NoContent();
        }

        [HttpDelete("{profileId}/deleteReview/{mediaId}")]
        public async Task<IActionResult> DeleteReview(Guid profileId, string mediaId) {
            var result = await _reviewService.DeleteReviewAsync(profileId, mediaId);
            if (!result) return NotFound();

            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{profileId}/reviews")]
        public async Task<IActionResult> GetReviewsByProfileId(Guid profileId, int page = 1, int pageSize = 10) {
            var reviews = await _reviewService.GetReviewsByProfileIdAsync(profileId, page, pageSize);
            return Ok(reviews);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetReviewsByProfileIds")]
        public async Task<IActionResult> GetReviewsByProfileIds([FromQuery] List<Guid> profileIds, int page = 1, int pageSize = 10) {
            var reviews = await _reviewService.GetReviewsByProfileIdsAsync(profileIds, page, pageSize);
            return Ok(reviews);
        }
    }

}
