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

        private string GetTokenFromRequest() {
            return Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{reviewId}/GetReviewById")]
        public async Task<IActionResult> GetReviewById(Guid reviewId) {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null) return NotFound();
            return Ok(review);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{reviewId}/GetReviewDetail")]
        public async Task<IActionResult> GetReviewDetail(Guid reviewId) {
            var token = GetTokenFromRequest();
            var reviewDetail = await _reviewService.GetReviewDetailAsync(reviewId, token);

            if (reviewDetail == null)
                return NotFound();

            return Ok(reviewDetail);
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{profileId}/AddReview")]
        public async Task<IActionResult> AddReview(Guid profileId, [FromBody] CreateReviewRequest request) {
            var token = GetTokenFromRequest();
            await _reviewService.AddReviewAsync(profileId, request, token);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{profileId}/DeleteReview/{mediaId}")]
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

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetMediaMainPageReviewInfo/{mediaId}")]
        public async Task<IActionResult> GetMediaMainPageReviewInfo(string mediaId) {
            var token = GetTokenFromRequest();
            var reviews = await _reviewService.GetMediaMainPageReviewInfo(mediaId, token);
            return Ok(reviews);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("GetMediaReviews/{mediaId}")]
        public async Task<IActionResult> GetMediaReviews(string mediaId, int page = 1, int pageSize = 10, string sortOption = "mostrecent") {
            var token = GetTokenFromRequest();
            var reviews = await _reviewService.GetMediaReviews(mediaId, page, pageSize, sortOption, token);
            return Ok(reviews);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{profileId}/{reviewId}/Like")]
        public async Task<IActionResult> AddLike(Guid profileId, Guid reviewId) {
            await _reviewService.AddLikeAsync(profileId, reviewId);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{profileId}/{reviewId}/Unlike")]
        public async Task<IActionResult> RemoveLike(Guid profileId, Guid reviewId) {
            await _reviewService.RemoveLikeAsync(profileId, reviewId);
            return NoContent();
        }

        [HttpGet("{reviewId}/LikeCount")]
        public async Task<IActionResult> GetLikeCount(Guid reviewId) {
            var count = await _reviewService.GetLikeCountAsync(reviewId);
            return Ok(count);
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("{profileId}/{reviewId}/HasLiked")]
        public async Task<IActionResult> HasUserLiked(Guid profileId, Guid reviewId) {
            var hasLiked = await _reviewService.HasUserLikedAsync(profileId, reviewId);
            return Ok(hasLiked);
        }

        // ────────────────────── Comments ──────────────────────

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("{profileId}/{reviewId}/Comment")]
        public async Task<IActionResult> AddComment(Guid profileId, Guid reviewId, [FromBody] CreateCommentRequest request) {
            await _reviewService.AddCommentAsync(profileId, reviewId, request);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("{commentId}/EditComment")]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] string newContent) {
            await _reviewService.UpdateCommentAsync(commentId, newContent);
            return NoContent();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpDelete("{commentId}/DeleteComment")]
        public async Task<IActionResult> DeleteComment(Guid commentId) {
            await _reviewService.DeleteCommentAsync(commentId);
            return NoContent();
        }

        [HttpGet("{reviewId}/TopLevelComments")]
        public async Task<IActionResult> GetTopLevelComments(Guid reviewId) {
            var token = GetTokenFromRequest();
            var comments = await _reviewService.GetTopLevelCommentsAsync(reviewId, token);
            return Ok(comments);
        }

        [HttpGet("{parentCommentId}/Replies")]
        public async Task<IActionResult> GetReplies(Guid parentCommentId) {
            var token = GetTokenFromRequest();
            var replies = await _reviewService.GetRepliesAsync(parentCommentId, token);
            return Ok(replies);
        }

        [HttpGet("{reviewId}/CommentCount")]
        public async Task<IActionResult> GetCommentCount(Guid reviewId) {
            var count = await _reviewService.GetCommentCountAsync(reviewId);
            return Ok(count);
        }

    }

}
