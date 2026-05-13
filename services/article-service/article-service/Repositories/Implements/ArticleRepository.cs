using article_service.Data;
using article_service.Models;
using article_service.Repositories.Interfaces;
using MongoDB.Driver;

namespace article_service.Repositories.Implements
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IMongoCollection<Article> _articles;

        public ArticleRepository(MongoDbContext context)
        {
            _articles = context.Articles;
        }

        public async Task<IEnumerable<Article>> GetAllAsync()
        {
            return await _articles
                .Find(FilterDefinition<Article>.Empty)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> SearchAsync(string? title, string? status, string? tags)
        {
            var builder = Builders<Article>.Filter;
            var filter = builder.Empty;

            if (!string.IsNullOrWhiteSpace(title))
                filter &= builder.Regex(a => a.Title, new MongoDB.Bson.BsonRegularExpression(title, "i"));

            if (!string.IsNullOrWhiteSpace(status))
                filter &= builder.Eq(a => a.Status, status);

            if (!string.IsNullOrWhiteSpace(tags))
                filter &= builder.Regex(a => a.Tags, new MongoDB.Bson.BsonRegularExpression(tags, "i"));

            return await _articles
                .Find(filter)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Article?> GetByIdAsync(string id)
        {
            return await _articles
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Article> AddAsync(Article article)
        {
            await _articles.InsertOneAsync(article);
            return article;
        }

        public async Task UpdateAsync(Article article)
        {
            await _articles.ReplaceOneAsync(a => a.Id == article.Id, article);
        }

        public async Task DeleteAsync(string id)
        {
            await _articles.DeleteOneAsync(a => a.Id == id);
        }
    }
}
