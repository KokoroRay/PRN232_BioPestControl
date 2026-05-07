namespace identity_service.DTOs.Requests
{
    public class UpdateStaffPermissionsRequest
    {
        public bool IsFullAccess { get; set; } = false;
        public List<int> PermissionIds { get; set; } = new();
    }
}
