using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace identity_service.Models
{
    /// <summary>
    /// Đại diện cho một nhân viên (Staff) trong hệ thống.
    /// Staff là một User có Role = "Staff", được Admin tạo ra và cấp quyền IAM.
    /// </summary>
    public class Staff
    {
        // Khóa chính
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK → bảng Users
        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        // Cấp toàn quyền Manager: true = có tất cả quyền UC14-UC20, false = chỉ các quyền được chọn
        public bool IsFullAccess { get; set; } = false;

        // Audit: Admin đã tạo / cập nhật
        public Guid? CreatedByAdminId { get; set; }
        public Guid? UpdatedByAdminId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation: Danh sách quyền cụ thể của Staff
        public ICollection<StaffPermission> StaffPermissions { get; set; } = new List<StaffPermission>();
    }
}
