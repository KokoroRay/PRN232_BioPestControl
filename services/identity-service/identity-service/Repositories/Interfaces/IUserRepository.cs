using identity_service.Models;

namespace identity_service.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<(List<User> Items, int TotalCount)> GetPagedCustomersAsync(DTOs.Requests.CustomerSearchRequest request);
        Task UpdateAsync(User user);
        Task<int> SaveChangesAsync();
    }
}
