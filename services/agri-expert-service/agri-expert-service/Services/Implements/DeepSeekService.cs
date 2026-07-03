using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using agri_expert_service.DTOs;
using agri_expert_service.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace agri_expert_service.Services.Implements
{
    public class DeepSeekService : IDeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _modelName;
        private readonly string _knowledgeBase;

        public DeepSeekService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            
            // Allow override via config, fallback to placeholder (user will supply later)
            _apiKey = (_configuration["DeepSeek:ApiKey"] ?? "YOUR_DEEPSEEK_API_KEY_HERE").Trim();
            _modelName = _configuration["DeepSeek:ModelName"] ?? "deepseek-chat";
            
            _httpClient.BaseAddress = new Uri("https://api.deepseek.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            // Load knowledge base for RAG
            var kbPath = Path.Combine(Directory.GetCurrentDirectory(), "knowledge.txt");
            _knowledgeBase = File.Exists(kbPath) ? File.ReadAllText(kbPath) : "No specific project knowledge available.";
        }

        public async Task<AiResponse> ChatAsync(string message)
        {
            try
            {
                // Simple RAG approach: inject the knowledge base directly into the system prompt
                var systemPrompt = $@"You are an agricultural expert AI assistant for the BioPestControl platform. 
Use the following project knowledge to answer the user's questions:

{_knowledgeBase}

If the user asks about something outside of this context, answer politely based on your general knowledge but emphasize the BioPestControl context when possible.";

                var payload = new
                {
                    model = _modelName,
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = message }
                    },
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var maskedKey = _apiKey.Length > 5 ? $"{_apiKey.Substring(0, 3)}...{_apiKey.Substring(_apiKey.Length - 2)}" : "EMPTY_OR_TOO_SHORT";
                    return new AiResponse { Success = false, ErrorMessage = $"DeepSeek API Error: {response.StatusCode} - {error}. (Debug Key: {maskedKey}, Length: {_apiKey.Length})" };
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                
                var reply = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                return new AiResponse { Success = true, Response = reply ?? string.Empty };
            }
            catch (Exception ex)
            {
                return new AiResponse { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<AiResponse> AnalyzeDiseaseAsync(string base64Image)
        {
            try
            {
                // Format base64 properly if it has the data:image prefix
                var base64Data = base64Image.Contains(",") ? base64Image.Split(',')[1] : base64Image;

                var systemPrompt = "You are an expert plant pathologist. Analyze the provided image of a plant/crop and identify any diseases, pests, or deficiencies. Provide a clear diagnosis, symptoms observed, and recommended organic/bio-pesticide treatments.";

                var payload = new
                {
                    model = _modelName,
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new 
                        { 
                            role = "user", 
                            content = new object[]
                            {
                                new { type = "text", text = "Please identify the disease in this plant image." },
                                new 
                                { 
                                    type = "image_url", 
                                    image_url = new { url = $"data:image/jpeg;base64,{base64Data}" } 
                                }
                            }
                        }
                    },
                    temperature = 0.5
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new AiResponse { Success = false, ErrorMessage = $"DeepSeek API Error: {response.StatusCode} - {error}" };
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                
                var reply = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                return new AiResponse { Success = true, Response = reply ?? string.Empty };
            }
            catch (Exception ex)
            {
                return new AiResponse { Success = false, ErrorMessage = ex.Message };
            }
        }
    }
}
