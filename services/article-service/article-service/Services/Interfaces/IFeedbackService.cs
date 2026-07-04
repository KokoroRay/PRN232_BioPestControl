using article_service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace article_service.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<Feedback>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Feedback>> GetAllAsync();
        Task<Feedback> CreateAsync(Feedback feedback);
        Task<bool> ReplyAsync(string id, string replyMessage, Guid staffId);
        Task<bool> DeleteAsync(string id);
    }
}
