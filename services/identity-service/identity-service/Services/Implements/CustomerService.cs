using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Models;
using identity_service.Repositories.Interfaces;
using identity_service.Services.Interfaces;

namespace identity_service.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly IUserRepository _userRepository;

        public CustomerService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<PagedResult<CustomerDto>>> GetPagedAsync(CustomerSearchRequest request)
        {
            var (items, totalCount) = await _userRepository.GetPagedCustomersAsync(request);

            var dtoList = items.Select(MapToDto).ToList();
            var pageSize = Math.Clamp(request.PageSize, 1, 100);
            var page = Math.Max(request.Page, 1);

            var result = new PagedResult<CustomerDto>
            {
                Items = dtoList,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ServiceResult<PagedResult<CustomerDto>>.Ok(result, $"Lấy danh sách thành công ({totalCount} khách hàng).");
        }

        public async Task<ServiceResult<CustomerDto>> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.Role != "Customer")
            {
                return ServiceResult<CustomerDto>.NotFound("Không tìm thấy khách hàng.");
            }

            return ServiceResult<CustomerDto>.Ok(MapToDto(user));
        }

        public async Task<ServiceResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.Role != "Customer")
            {
                return ServiceResult<CustomerDto>.NotFound("Không tìm thấy khách hàng.");
            }

            if (request.FullName != null) user.FullName = request.FullName.Trim();
            if (request.PhoneNumber != null) user.PhoneNumber = request.PhoneNumber.Trim();
            if (request.AvatarUrl != null) user.AvatarUrl = request.AvatarUrl.Trim();

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return ServiceResult<CustomerDto>.Ok(MapToDto(user), "Cập nhật khách hàng thành công.");
        }

        public async Task<ServiceResult<object>> UpdateStatusAsync(Guid id, UpdateCustomerStatusRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.Role != "Customer")
            {
                return ServiceResult<object>.NotFound("Không tìm thấy khách hàng.");
            }

            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var statusText = request.IsActive ? "mở khóa" : "khóa";
            return ServiceResult<object>.Ok(null!, $"Đã {statusText} khách hàng thành công.");
        }

        private static CustomerDto MapToDto(User user) => new()
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            PhoneNumber = user.PhoneNumber,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
