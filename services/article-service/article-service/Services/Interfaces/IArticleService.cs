using article_service.DTOs.Requests;
using article_service.DTOs.Responses;

namespace article_service.Services.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleResponse>> GetAllAsync();
        Task<IEnumerable<ArticleResponse>> SearchAsync(string? title, string? status, string? tags);
        Task<ArticleResponse?> GetByIdAsync(string id);
        Task<ArticleResponse> AddAsync(CreateArticleRequest request);
        Task<bool> UpdateAsync(string id, UpdateArticleRequest request);
        Task<bool> DeleteAsync(string id);
    }
}
