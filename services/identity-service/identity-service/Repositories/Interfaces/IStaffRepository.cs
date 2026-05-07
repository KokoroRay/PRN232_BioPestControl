using identity_service.DTOs;
using identity_service.Models;

namespace identity_service.Repositories.Interfaces
{
    // Interface định nghĩa các thao tác dữ liệu cho Staff.
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
