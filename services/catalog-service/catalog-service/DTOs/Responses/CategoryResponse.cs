namespace catalog_service.DTOs.Responses
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? CreatedByAdminId { get; set; }
        public int? ManagedByStaffId { get; set; }
    }
}
