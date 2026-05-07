using System.ComponentModel.DataAnnotations;

namespace agri_expert_service.DTOs.Requests
{
    public class UpdateChemicalRequest
    {
        [Required(ErrorMessage = "Tên hóa chất là bắt buộc.")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? VietnameseName { get; set; }

        [MaxLength(50)]
        public string? CasNumber { get; set; }

        [MaxLength(100)]
        public string? ChemicalGroup { get; set; }

        [MaxLength(100)]
        public string? ChemicalFormula { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? ToxicityLevel { get; set; }

        [MaxLength(500)]
        public string? UsageMethod { get; set; }

        [MaxLength(1000)]
        public string? SafetyNotes { get; set; }

        [MaxLength(500)]
        public string? TargetCrops { get; set; }

        [MaxLength(500)]
        public string? TargetPests { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
