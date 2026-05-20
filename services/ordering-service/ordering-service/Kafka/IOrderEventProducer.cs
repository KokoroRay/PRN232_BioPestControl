using ordering_service.Kafka;

namespace ordering_service.Kafka
{
    public interface IOrderEventProducer
    {
        Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent);
    }
}
