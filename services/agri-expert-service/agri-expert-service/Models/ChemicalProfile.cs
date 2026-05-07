using System.ComponentModel.DataAnnotations;

namespace agri_expert_service.Models
{
    /// <summary>
    /// Thông tin hồ sơ hóa chất nông nghiệp (Manage Chemical Safety - UC20)
    /// </summary>
    public class ChemicalProfile
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Tên hóa chất (tiếng Anh)</summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>Tên hóa chất tiếng Việt / tên thông thường</summary>
        [MaxLength(200)]
        public string? VietnameseName { get; set; }

        /// <summary>Số CAS định danh hóa chất quốc tế</summary>
        [MaxLength(50)]
        public string? CasNumber { get; set; }

        /// <summary>Nhóm hóa chất: Thuốc trừ sâu, Thuốc diệt nấm, Thuốc diệt cỏ, Điều hòa sinh trưởng...</summary>
        [MaxLength(100)]
        public string? ChemicalGroup { get; set; }

        /// <summary>Công thức hóa học</summary>
        [MaxLength(100)]
        public string? ChemicalFormula { get; set; }

        /// <summary>Mô tả công dụng / hướng dẫn sử dụng</summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>Mức độ độc hại (WHO class: Ia, Ib, II, III, U…)</summary>
        [MaxLength(20)]
        public string? ToxicityLevel { get; set; }

        /// <summary>Cách dùng: phun, rải, hòa nước...</summary>
        [MaxLength(500)]
        public string? UsageMethod { get; set; }

        /// <summary>Ghi chú an toàn / cảnh báo</summary>
        [MaxLength(1000)]
        public string? SafetyNotes { get; set; }

        /// <summary>Danh sách cây trồng áp dụng (phân tách bằng dấu phẩy)</summary>
        [MaxLength(500)]
        public string? TargetCrops { get; set; }

        /// <summary>Đối tượng phòng trừ (sâu, bệnh, cỏ dại...)</summary>
        [MaxLength(500)]
        public string? TargetPests { get; set; }

        /// <summary>Hiện đang hoạt động / bị ẩn</summary>
        public bool IsActive { get; set; } = true;

        // Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
