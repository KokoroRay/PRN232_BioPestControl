using Confluent.Kafka;
using System.Text.Json;

namespace ordering_service.Kafka
{
    public class OrderEventProducer : IOrderEventProducer, IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;
        private readonly ILogger<OrderEventProducer> _logger;

        public OrderEventProducer(IConfiguration config, ILogger<OrderEventProducer> logger)
        {
            _logger = logger;
            _topic  = config["Kafka:OrderPlacedTopic"] ?? "order.placed";

            var producerConfig = new ProducerConfig
            {
                BootstrapServers        = config["Kafka:BootstrapServers"] ?? "localhost:9092",
                MessageTimeoutMs        = 5000,
                RequestTimeoutMs        = 5000,
                EnableDeliveryReports   = true
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task PublishOrderPlacedAsync(OrderPlacedEvent orderEvent)
        {
            try
            {
                var message = new Message<string, string>
                {
                    Key   = orderEvent.OrderId.ToString(),
                    Value = JsonSerializer.Serialize(orderEvent)
                };

                var result = await _producer.ProduceAsync(_topic, message);
                _logger.LogInformation(
                    "[Kafka] Published OrderPlaced — OrderId={OrderId}, Partition={Partition}, Offset={Offset}",
                    orderEvent.OrderId, result.Partition, result.Offset);
            }
            catch (Exception ex)
            {
                // Kafka failure không block đặt hàng, chỉ log warning
                _logger.LogWarning(ex,
                    "[Kafka] Failed to publish OrderPlaced for OrderId={OrderId}. Inventory deduction may be delayed.",
                    orderEvent.OrderId);
            }
        }

        public void Dispose() => _producer?.Dispose();
    }
}
