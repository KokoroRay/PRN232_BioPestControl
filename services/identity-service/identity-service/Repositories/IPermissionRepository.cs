using identity_service.Models;

namespace identity_service.Repositories
{
    public interface IPermissionRepository
    {
        Task<List<Permission>> GetAllActiveAsync();
        Task<List<Permission>> GetByIdsAsync(IEnumerable<int> ids);
        Task<Permission?> GetByIdAsync(int id);
        Task<bool> AllExistAsync(IEnumerable<int> ids);
    }
}
