using catalog_service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace catalog_service.Repositories.Interfaces
{
    public interface ICropRepository
    {
        Task<IEnumerable<Crop>> GetAllAsync(bool includeInactive = false);
        Task<Crop?> GetByIdAsync(int id);
        Task<Crop?> GetBySlugAsync(string slug);
        Task<Crop> CreateAsync(Crop crop);
        Task<Crop> UpdateAsync(Crop crop);
        Task DeleteAsync(int id);
    }
}
