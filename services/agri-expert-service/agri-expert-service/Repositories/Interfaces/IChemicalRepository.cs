using agri_expert_service.DTOs.Requests;
using agri_expert_service.DTOs.Responses;

namespace agri_expert_service.Repositories.Interfaces
{
    public interface IChemicalRepository
    {
        Task<(List<agri_expert_service.Models.ChemicalProfile> Items, int TotalCount)> GetPagedAsync(ChemicalSearchRequest request);
        Task<agri_expert_service.Models.ChemicalProfile?> GetByIdAsync(int id);
        Task<agri_expert_service.Models.ChemicalProfile?> GetByCasNumberAsync(string casNumber);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> ExistsByCasAsync(string casNumber, int? excludeId = null);
        Task<List<string>> GetDistinctGroupsAsync();
        Task AddAsync(agri_expert_service.Models.ChemicalProfile chemical);
        Task UpdateAsync(agri_expert_service.Models.ChemicalProfile chemical);
        Task DeleteAsync(agri_expert_service.Models.ChemicalProfile chemical);
        Task<int> SaveChangesAsync();
    }
}
