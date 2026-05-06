namespace inventory_service.DTOs.Responses
{
    /// <summary>
    /// Thông tin một dòng nhập kho
    /// </summary>
    public class WarehouseImportResponse
    {
        public int Id { get; set; }
        public string BatchCode { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductSKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int QuantityImported { get; set; }
        public decimal ImportPrice { get; set; }
        public string? SupplierName { get; set; }
        public string? Note { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public Guid ImportedByUserId { get; set; }
        public string? ImportedByUserName { get; set; }
        public DateTime ImportedAt { get; set; }
    }

    /// <summary>
    /// Tóm tắt một phiếu nhập kho (batch) — gom nhiều dòng sản phẩm
    /// </summary>
    public class ImportBatchSummaryResponse
    {
        public string BatchCode { get; set; } = string.Empty;
        public DateTime ImportedAt { get; set; }
        public string? ImportedByUserName { get; set; }
        public string? SupplierName { get; set; }
        public int TotalProducts { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalImportValue { get; set; }
        public List<WarehouseImportResponse> Items { get; set; } = new();
    }
}
