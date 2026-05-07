namespace agri_expert_service.DTOs.Responses
{
    public class ChemicalDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? VietnameseName { get; set; }
        public string? CasNumber { get; set; }
        public string? ChemicalGroup { get; set; }
        public string? ChemicalFormula { get; set; }
        public string? Description { get; set; }
        public string? ToxicityLevel { get; set; }
        public string? UsageMethod { get; set; }
        public string? SafetyNotes { get; set; }
        public string? TargetCrops { get; set; }
        public string? TargetPests { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
