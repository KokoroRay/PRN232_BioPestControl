using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace identity_service.Models
{
    public class StaffPermission
    {
        // Khóa chính tự sinh
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // FK → Staff
        [Required]
        public Guid StaffId { get; set; }

        [ForeignKey(nameof(StaffId))]
        public Staff Staff { get; set; } = null!;

        // FK → Permission
        [Required]
        public int PermissionId { get; set; }

        [ForeignKey(nameof(PermissionId))]
        public Permission Permission { get; set; } = null!;

        // Thời điểm Admin cấp quyền này
        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

        // ID Admin đã cấp quyền
        public Guid? GrantedByAdminId { get; set; }
    }
}
