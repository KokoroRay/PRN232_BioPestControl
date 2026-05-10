using System.Threading.Tasks;

namespace catalog_service.Services.Interfaces
{
    public interface IIdentityServiceClient
    {
        Task<string?> GetUserNameAsync(int userId);
    }
}
