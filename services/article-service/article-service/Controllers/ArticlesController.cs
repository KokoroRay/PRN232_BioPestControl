using article_service.DTOs.Requests;
using article_service.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace article_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public ArticlesController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        /// <summary>
        /// Get all articles or search/filter by query parameters.
        /// Accessible by: Guest, Customer, Staff, Admin
        /// </summary>
        /// <param name="title">Search by article title (contains, case-insensitive)</param>
        /// <param name="status">Filter by status (Draft, Published, Archived)</param>
        /// <param name="tags">Filter by tag keyword (contains, case-insensitive)</param>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? title,
            [FromQuery] string? status,
            [FromQuery] string? tags)
        {
            // If any filter/search param is provided, use SearchAsync
            if (!string.IsNullOrWhiteSpace(title) ||
                !string.IsNullOrWhiteSpace(status) ||
                !string.IsNullOrWhiteSpace(tags))
            {
                var searchResults = await _articleService.SearchAsync(title, status, tags);
                return Ok(searchResults);
            }

            var articles = await _articleService.GetAllAsync();
            return Ok(articles);
        }

        /// <summary>
        /// Get article detail by ID (MongoDB ObjectId string).
        /// Accessible by: Guest, Customer, Staff, Admin
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var article = await _articleService.GetByIdAsync(id);
            if (article == null) return NotFound(new { message = $"Article with id {id} not found." });

            return Ok(article);
        }

        /// <summary>
        /// Create a new article.
        /// Accessible by: Staff, Admin
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateArticleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdArticle = await _articleService.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = createdArticle.Id }, createdArticle);
        }

        /// <summary>
        /// Update an existing article.
        /// Accessible by: Staff, Admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateArticleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updated = await _articleService.UpdateAsync(id, request);
            if (!updated) return NotFound(new { message = $"Article with id {id} not found." });

            return NoContent();
        }

        /// <summary>
        /// Delete an article.
        /// Accessible by: Staff, Admin
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _articleService.DeleteAsync(id);
            if (!deleted) return NotFound(new { message = $"Article with id {id} not found." });

            return NoContent();
        }
    }
}
