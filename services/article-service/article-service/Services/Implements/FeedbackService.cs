using article_service.Data;
using article_service.Models;
using article_service.Services.Interfaces;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace article_service.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly MongoDbContext _context;

        public FeedbackService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetByProductIdAsync(int productId)
        {
            return await _context.Feedbacks.Find(f => f.ProductId == productId).SortByDescending(f => f.CreatedAt).ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetAllAsync()
        {
            return await _context.Feedbacks.Find(_ => true).SortByDescending(f => f.CreatedAt).ToListAsync();
        }

        public async Task<Feedback> CreateAsync(Feedback feedback)
        {
            await _context.Feedbacks.InsertOneAsync(feedback);
            return feedback;
        }

        public async Task<bool> ReplyAsync(string id, string replyMessage, Guid staffId)
        {
            var update = Builders<Feedback>.Update
                .Set(f => f.ReplyMessage, replyMessage)
                .Set(f => f.RepliedAt, DateTime.UtcNow)
                .Set(f => f.RepliedByStaffId, staffId);

            var result = await _context.Feedbacks.UpdateOneAsync(f => f.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _context.Feedbacks.DeleteOneAsync(f => f.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
