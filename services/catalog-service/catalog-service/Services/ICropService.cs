using catalog_service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace catalog_service.Services.Interfaces
{
    public interface ICropService
    {
        Task<IEnumerable<CropResponse>> GetAllAsync(bool includeInactive = false);
        Task<CropProfileResponse?> GetByIdAsync(int id);
        Task<CropProfileResponse?> GetBySlugAsync(string slug);
    }
}
