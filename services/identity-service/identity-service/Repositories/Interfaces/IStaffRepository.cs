using identity_service.DTOs.Requests;
using identity_service.Models;

namespace identity_service.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<(List<Staff> Items, int TotalCount)> GetPagedAsync(StaffSearchRequest request);
        Task<Staff?> GetByIdAsync(Guid staffId);
        Task<Staff?> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsByUserIdAsync(Guid userId);
        Task AddAsync(Staff staff);
        Task UpdateAsync(Staff staff);
        Task DeleteAsync(Staff staff);
        Task ClearPermissionsAsync(Guid staffId);
        Task AddPermissionsAsync(IEnumerable<StaffPermission> permissions);
        Task<int> SaveChangesAsync();
    }
}
