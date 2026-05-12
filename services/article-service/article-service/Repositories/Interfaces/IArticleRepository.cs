using article_service.Models;

namespace article_service.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetAllAsync();
        Task<IEnumerable<Article>> SearchAsync(string? title, string? status, string? tags);
        Task<Article?> GetByIdAsync(string id);
        Task<Article> AddAsync(Article article);
        Task UpdateAsync(Article article);
        Task DeleteAsync(string id);
    }
}
