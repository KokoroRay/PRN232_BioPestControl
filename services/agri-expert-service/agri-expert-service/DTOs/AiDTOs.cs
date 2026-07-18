using System.ComponentModel.DataAnnotations;

namespace agri_expert_service.DTOs
{
    public class AiChatRequest
    {
        [Required]
        public string Message { get; set; } = string.Empty;

        public List<string> Images { get; set; } = new List<string>();
    }

    public class AiResponse
    {
        public bool Success { get; set; }
        public string Response { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
