using engagement_service.Data;
using engagement_service.Models;
using engagement_service.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace engagement_service.Repositories.Implements
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly EngagementDbContext _context;

        public ArticleRepository(EngagementDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _context.Articles
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> SearchAsync(string? title, string? status, string? tags)
        {
            var query = _context.Articles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(a => a.Title.Contains(title));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(a => a.Status == status);

            if (!string.IsNullOrWhiteSpace(tags))
                query = query.Where(a => a.Tags != null && a.Tags.Contains(tags));

            return await query
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(int id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<Article> AddAsync(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            return article;
        }

        public async Task UpdateAsync(Article article)
        {
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Article article)
        {
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
        }
    }
}
