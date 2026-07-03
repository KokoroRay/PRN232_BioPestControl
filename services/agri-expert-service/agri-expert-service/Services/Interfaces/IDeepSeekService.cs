using System.Threading.Tasks;
using agri_expert_service.DTOs;

namespace agri_expert_service.Services.Interfaces
{
    public interface IDeepSeekService
    {
        Task<AiResponse> ChatAsync(string message);
        Task<AiResponse> AnalyzeDiseaseAsync(string base64Image);
    }
}
