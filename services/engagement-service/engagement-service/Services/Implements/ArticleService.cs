using engagement_service.DTOs.Requests;
using engagement_service.DTOs.Responses;
using engagement_service.Models;
using engagement_service.Repositories.Interfaces;
using engagement_service.Services.Interfaces;

namespace engagement_service.Services.Implements
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _repository;

        public ArticleService(IArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ArticleResponse>> GetAllAsync()
        {
            var articles = await _repository.GetAllAsync();
            return articles.Select(MapToResponse);
        }

        public async Task<IEnumerable<ArticleResponse>> SearchAsync(string? title, string? status, string? tags)
        {
            var articles = await _repository.SearchAsync(title, status, tags);
            return articles.Select(MapToResponse);
        }

        public async Task<ArticleResponse?> GetByIdAsync(int id)
        {
            var article = await _repository.GetByIdAsync(id);
            if (article == null) return null;
            return MapToResponse(article);
        }

        public async Task<ArticleResponse> AddAsync(CreateArticleRequest request)
        {
            var article = new Article
            {
                Title = request.Title,
                Content = request.Content,
                Summary = request.Summary,
                ThumbnailUrl = request.ThumbnailUrl,
                Status = request.Status,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow,
                CreatedByStaffId = request.CreatedByStaffId,
                ManagedByAdminId = request.ManagedByAdminId
            };

            var addedArticle = await _repository.AddAsync(article);
            return MapToResponse(addedArticle);
        }

        public async Task<bool> UpdateAsync(int id, UpdateArticleRequest request)
        {
            var existingArticle = await _repository.GetByIdAsync(id);
            if (existingArticle == null) return false;

            existingArticle.Title = request.Title;
            existingArticle.Content = request.Content;
            existingArticle.Summary = request.Summary;
            existingArticle.ThumbnailUrl = request.ThumbnailUrl;
            existingArticle.Status = request.Status;
            existingArticle.Tags = request.Tags;
            existingArticle.ManagedByAdminId = request.ManagedByAdminId;
            existingArticle.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingArticle);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existingArticle = await _repository.GetByIdAsync(id);
            if (existingArticle == null) return false;

            await _repository.DeleteAsync(existingArticle);
            return true;
        }

        private static ArticleResponse MapToResponse(Article article)
        {
            return new ArticleResponse
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                Summary = article.Summary,
                ThumbnailUrl = article.ThumbnailUrl,
                Status = article.Status,
                Tags = article.Tags,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt,
                CreatedByStaffId = article.CreatedByStaffId,
                ManagedByAdminId = article.ManagedByAdminId
            };
        }
    }
}
