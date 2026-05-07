using agri_expert_service.DTOs.Requests;
using agri_expert_service.DTOs.Responses;

namespace agri_expert_service.Services.Interfaces
{
    public interface IChemicalService
    {
        Task<ServiceResult<PagedResult<ChemicalDto>>> GetPagedAsync(ChemicalSearchRequest request);
        Task<ServiceResult<ChemicalDto>> GetByIdAsync(int id);
        Task<ServiceResult<List<string>>> GetGroupsAsync();
        Task<ServiceResult<ChemicalDto>> CreateAsync(CreateChemicalRequest request);
        Task<ServiceResult<ChemicalDto>> UpdateAsync(int id, UpdateChemicalRequest request);
        Task<ServiceResult<object>> DeleteAsync(int id);
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
