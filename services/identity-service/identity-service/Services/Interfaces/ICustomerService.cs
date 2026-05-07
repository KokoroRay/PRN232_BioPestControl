using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;

namespace identity_service.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ServiceResult<PagedResult<CustomerDto>>> GetPagedAsync(CustomerSearchRequest request);
        Task<ServiceResult<CustomerDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request);
        Task<ServiceResult<object>> UpdateStatusAsync(Guid id, UpdateCustomerStatusRequest request);
    }
}
