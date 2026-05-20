using System.Text.Json;
using catalog_service.Services.Interfaces;

namespace catalog_service.Services.Implements
{
    public class AgriExpertServiceClient : IAgriExpertServiceClient
    {
        private readonly HttpClient _httpClient;

        public AgriExpertServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetChemicalNameAsync(int chemicalProfileId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/chemicals/{chemicalProfileId}");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var payload = JsonSerializer.Deserialize<ApiResponse<ChemicalDto>>(
                    content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return payload?.Data?.Name;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ExistsChemicalProfileAsync(int chemicalProfileId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/chemicals/{chemicalProfileId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private class ApiResponse<T>
        {
            public T? Data { get; set; }
        }

        private class ChemicalDto
        {
            public string Name { get; set; } = string.Empty;
        }
    }
}
