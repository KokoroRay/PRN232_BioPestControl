namespace catalog_service.DTOs.Responses
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public string? ImageUrl { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? ChemicalProfileId { get; set; }
        public string? ChemicalName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedByAdminId { get; set; }
        public string? CreatedByAdminName { get; set; }
        public int? ManagedByStaffId { get; set; }
        public string? ManagedByStaffName { get; set; }
        public List<int> CropIds { get; set; } = new List<int>();
    }
}
