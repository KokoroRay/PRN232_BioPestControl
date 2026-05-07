using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;

namespace identity_service.Services.Interfaces
{
    public interface IStaffService
    {
        Task<ServiceResult<PagedResult<StaffDto>>> GetPagedAsync(StaffSearchRequest request);
        Task<ServiceResult<StaffDto>> GetByIdAsync(Guid staffId);
        Task<ServiceResult<StaffDto>> CreateAsync(CreateStaffRequest request, Guid adminId);
        Task<ServiceResult<StaffDto>> UpdateAsync(Guid staffId, UpdateStaffRequest request, Guid adminId);
        Task<ServiceResult<object>> DeleteAsync(Guid staffId);
        Task<ServiceResult<object>> UpdateStatusAsync(Guid staffId, UpdateStaffStatusRequest request, Guid adminId);
        Task<ServiceResult<StaffDto>> UpdatePermissionsAsync(Guid staffId, UpdateStaffPermissionsRequest request, Guid adminId);
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public int StatusCode { get; set; } = 200;

        public static ServiceResult<T> Ok(T data, string message = "Thành công.")
            => new() { Success = true, Message = message, Data = data, StatusCode = 200 };

        public static ServiceResult<T> Created(T data, string message = "Tạo thành công.")
            => new() { Success = true, Message = message, Data = data, StatusCode = 201 };

        public static ServiceResult<T> Fail(string message, int statusCode = 400)
            => new() { Success = false, Message = message, StatusCode = statusCode };

        public static ServiceResult<T> NotFound(string message = "Không tìm thấy.")
            => new() { Success = false, Message = message, StatusCode = 404 };

        public static ServiceResult<T> Conflict(string message)
            => new() { Success = false, Message = message, StatusCode = 409 };
    }
}
