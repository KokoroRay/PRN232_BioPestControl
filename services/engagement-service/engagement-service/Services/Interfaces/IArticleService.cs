using engagement_service.DTOs.Requests;
using engagement_service.DTOs.Responses;

namespace engagement_service.Services.Interfaces
{
    public interface IArticleService
    {
        Task<IEnumerable<ArticleResponse>> GetAllAsync();
        Task<IEnumerable<ArticleResponse>> SearchAsync(string? title, string? status, string? tags);
        Task<ArticleResponse?> GetByIdAsync(int id);
        Task<ArticleResponse> AddAsync(CreateArticleRequest request);
        Task<bool> UpdateAsync(int id, UpdateArticleRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
