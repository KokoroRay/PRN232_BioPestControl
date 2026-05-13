using Confluent.Kafka;
using inventory_service.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace inventory_service.Kafka
{
    /// <summary>
    /// Background hosted service — lắng nghe topic "order.placed" từ Kafka
    /// và tự động trừ kho khi có đơn hàng mới.
    /// </summary>
    public class KafkaConsumerHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<KafkaConsumerHostedService> _logger;

        public KafkaConsumerHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration config,
            ILogger<KafkaConsumerHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _config       = config;
            _logger       = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bootstrapServers = _config["Kafka:BootstrapServers"] ?? "localhost:9092";
            var topic            = _config["Kafka:OrderPlacedTopic"]  ?? "order.placed";
            var groupId          = _config["Kafka:ConsumerGroupId"]    ?? "inventory-consumer-group";

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId          = groupId,
                AutoOffsetReset  = AutoOffsetReset.Earliest,
                EnableAutoCommit = false   // Manual commit sau khi xử lý thành công
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(topic);

            _logger.LogInformation("[Kafka] Inventory consumer started. Subscribed to topic '{Topic}'", topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    if (consumeResult?.Message?.Value == null) continue;

                    _logger.LogInformation("[Kafka] Received message. Partition={P}, Offset={O}",
                        consumeResult.Partition, consumeResult.Offset);

                    var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(
                        consumeResult.Message.Value,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (orderEvent != null)
                        await DeductStockAsync(orderEvent, stoppingToken);

                    // Manual commit: chỉ commit sau khi xử lý thành công
                    consumer.Commit(consumeResult);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "[Kafka] Consume error: {Reason}", ex.Error.Reason);
                    await Task.Delay(3000, stoppingToken); // Back-off ngắn trước khi thử lại
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Kafka] Unexpected error while processing OrderPlaced event");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            consumer.Close();
            _logger.LogInformation("[Kafka] Inventory consumer stopped.");
        }

        private async Task DeductStockAsync(OrderPlacedEvent orderEvent, CancellationToken ct)
        {
            // Tạo scope riêng vì DbContext là scoped, BackgroundService là singleton
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

            foreach (var item in orderEvent.Items)
            {
                var product = await db.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId, ct);

                if (product == null)
                {
                    _logger.LogWarning("[Inventory] ProductId={ProductId} not found in inventory. OrderId={OrderId}",
                        item.ProductId, orderEvent.OrderId);
                    continue;
                }

                var before = product.StockQuantity;
                product.StockQuantity -= item.Quantity;
                product.UpdatedAt      = DateTime.UtcNow;

                if (product.StockQuantity < 0)
                {
                    _logger.LogWarning(
                        "[Inventory] StockQuantity for ProductId={ProductId} went negative ({Before} - {Qty} = {After}). OrderId={OrderId}",
                        item.ProductId, before, item.Quantity, product.StockQuantity, orderEvent.OrderId);
                }

                if (product.StockQuantity <= product.LowStockThreshold)
                {
                    _logger.LogWarning(
                        "[Inventory] LOW STOCK ALERT — ProductId={ProductId} '{Name}': StockQuantity={Stock} <= Threshold={Threshold}",
                        product.Id, product.Name, product.StockQuantity, product.LowStockThreshold);
                }
            }

            await db.SaveChangesAsync(ct);

            _logger.LogInformation("[Inventory] Stock deducted for OrderId={OrderId}. {Count} product(s) updated.",
                orderEvent.OrderId, orderEvent.Items.Count);
        }
    }
}
