namespace ordering_service.Kafka
{
    /// <summary>
    /// Event được publish lên Kafka khi Customer đặt hàng thành công.
    /// inventory-service sẽ subscribe topic này để tự động trừ kho.
    /// </summary>
    public class OrderPlacedEvent
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
        public List<OrderItemEvent> Items { get; set; } = new();
    }

    public class OrderItemEvent
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
