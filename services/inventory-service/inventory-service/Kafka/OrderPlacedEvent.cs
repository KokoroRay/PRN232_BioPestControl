namespace inventory_service.Kafka
{
    /// <summary>
    /// Event nhận được từ Kafka khi ordering-service tạo đơn hàng thành công.
    /// inventory-service dùng event này để tự động trừ kho.
    /// </summary>
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime PlacedAt { get; set; }
        public List<OrderItemEvent> Items { get; set; } = new();
    }

    public class OrderItemEvent
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
