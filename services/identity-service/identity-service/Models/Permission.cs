using System.ComponentModel.DataAnnotations;

namespace identity_service.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string DisplayName { get; set; } = string.Empty;

        // Mô tả chi tiết hơn về quyền này
        [MaxLength(500)]
        public string? Description { get; set; }

        // Nhóm quyền theo Use Case (VD: "UC14 - Quản lý khách hàng")
        // Dùng để group khi hiển thị UI checkbox
        [Required]
        [MaxLength(100)]
        public string GroupCode { get; set; } = string.Empty;

        // Tên hiển thị của nhóm
        [Required]
        [MaxLength(200)]
        public string GroupName { get; set; } = string.Empty;

        // Thứ tự hiển thị trong nhóm (để sắp xếp UI)
        public int DisplayOrder { get; set; } = 0;

        // Trạng thái: quyền có đang được dùng không (có thể disable một quyền tạm thời)
        public bool IsActive { get; set; } = true;

        // Navigation: Danh sách StaffPermission sử dụng quyền này
        public ICollection<StaffPermission> StaffPermissions { get; set; } = new List<StaffPermission>();
    }
}
