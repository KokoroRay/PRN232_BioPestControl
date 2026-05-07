namespace identity_service.DTOs
{
    /// <summary>
    /// DTO thông tin một quyền đơn lẻ
    /// </summary>
    public class PermissionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO nhóm quyền — dùng để hiển thị checkbox UI theo từng UC
    /// </summary>
    public class PermissionGroupDto
    {
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public List<PermissionDto> Permissions { get; set; } = new();
    }
}
