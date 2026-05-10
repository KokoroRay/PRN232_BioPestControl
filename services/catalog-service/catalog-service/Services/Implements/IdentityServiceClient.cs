using catalog_service.Services.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace catalog_service.Services.Implements
{
    public class IdentityServiceClient : IIdentityServiceClient
    {
        private readonly HttpClient _httpClient;

        public IdentityServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetUserNameAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return user != null ? $"{user.FirstName} {user.LastName}".Trim() : null;
                }
            }
            catch
            {
                // Log exception if needed
            }
            return null;
        }
        
        private class UserDto
        {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
        }
    }
}
