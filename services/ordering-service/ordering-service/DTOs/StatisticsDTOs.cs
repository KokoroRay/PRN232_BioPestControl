namespace ordering_service.DTOs
{
    public class DashboardStatsDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalSoldQuantity { get; set; }
        public int TotalLinkedProducts { get; set; } // Số lượng sản phẩm khác nhau đã được bán
    }

    public class RevenueStatDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }

    public class SoldProductStatDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class StatsFilterRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
