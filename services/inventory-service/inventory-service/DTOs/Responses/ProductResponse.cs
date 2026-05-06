namespace inventory_service.DTOs.Responses
{
    /// <summary>
    /// Thông tin tồn kho sản phẩm — dành cho Staff (xem tồn kho)
    /// </summary>
    public class ProductStockResponse
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Thông tin chi tiết sản phẩm kèm lịch sử nhập — dành cho Admin
    /// </summary>
    public class ProductDetailResponse : ProductStockResponse
    {
        public List<WarehouseImportResponse> ImportHistory { get; set; } = new();
    }
}
