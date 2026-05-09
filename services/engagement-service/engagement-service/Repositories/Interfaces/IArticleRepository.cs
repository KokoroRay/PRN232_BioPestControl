using engagement_service.Models;

namespace engagement_service.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetAllAsync();
        Task<IEnumerable<Article>> SearchAsync(string? title, string? status, string? tags);
        Task<Article?> GetByIdAsync(int id);
        Task<Article> AddAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeleteAsync(Article article);
    }
}
