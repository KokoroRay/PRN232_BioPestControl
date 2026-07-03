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
            _apiKey = (_configuration["OpenAI:ApiKey"] ?? "YOUR_OPENAI_API_KEY_HERE").Trim();
            _modelName = _configuration["OpenAI:ModelName"] ?? "gpt-4o-mini";
            
            var baseUrl = _configuration["OpenAI:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "https://api.openai.com/v1/";
            }
            if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }
            
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            // Load knowledge base for RAG
            var kbPath = Path.Combine(Directory.GetCurrentDirectory(), "knowledge.txt");
            _knowledgeBase = File.Exists(kbPath) ? File.ReadAllText(kbPath) : "No specific project knowledge available.";
        }

        public async Task<AiResponse> ChatAsync(AiChatRequest request)
        {
            try
            {
                // Simple RAG approach: inject the knowledge base directly into the system prompt
                var systemPrompt = $@"You are an agricultural expert AI assistant for the BioPestControl platform. 
Use the following project knowledge to answer the user's questions:

{_knowledgeBase}

If the user asks about something outside of this context, answer politely based on your general knowledge but emphasize the BioPestControl context when possible.";

                object userContent;
                if (request.Images != null && request.Images.Count > 0)
                {
                    var contentList = new System.Collections.Generic.List<object>();
                    if (!string.IsNullOrWhiteSpace(request.Message))
                    {
                        contentList.Add(new { type = "text", text = request.Message });
                    }
                    else
                    {
                        contentList.Add(new { type = "text", text = "Please analyze these images." });
                    }
                    
                    foreach(var img in request.Images)
                    {
                        var base64Data = img.Contains(",") ? img.Split(',')[1] : img;
                        contentList.Add(new 
                        { 
                            type = "image_url", 
                            image_url = new { url = $"data:image/jpeg;base64,{base64Data}" } 
                        });
                    }
                    userContent = contentList;
                }
                else
                {
                    userContent = request.Message;
                }

                var payload = new
                {
                    model = _modelName,
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new { role = "user", content = userContent }
                    },
                    temperature = 0.7
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    var maskedKey = _apiKey.Length > 5 ? $"{_apiKey.Substring(0, 3)}...{_apiKey.Substring(_apiKey.Length - 2)}" : "EMPTY_OR_TOO_SHORT";
                    var fullUrl = _httpClient.BaseAddress + "chat/completions";
                    return new AiResponse { Success = false, ErrorMessage = $"API Proxy Error at {fullUrl}: {response.StatusCode} - {error}. (Debug Key: {maskedKey}, Length: {_apiKey.Length})" };
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
