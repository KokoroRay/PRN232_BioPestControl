using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace engagement_service.Services
{
    /// <summary>
    /// Gọi order-service: GET BaseUrl/EligibilityRelativePath với query customerId, orderId, productId.
    /// Chi tiết contract: INTEGRATION.md.
    /// </summary>
    public class HttpOrderPurchaseEligibilityService : IPurchaseEligibilityService
    {
        public const string HttpClientName = nameof(HttpOrderPurchaseEligibilityService);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<OrderServiceOptions> _options;
        private readonly ILogger<HttpOrderPurchaseEligibilityService> _logger;

        public HttpOrderPurchaseEligibilityService(
            IHttpClientFactory httpClientFactory,
            IOptions<OrderServiceOptions> options,
            ILogger<HttpOrderPurchaseEligibilityService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
            _logger = logger;
        }

        public async Task<bool> HasDeliveredProductInOrderAsync(
            int customerId,
            int orderId,
            int productId,
            CancellationToken cancellationToken = default)
        {
            var o = _options.Value;
            if (string.IsNullOrWhiteSpace(o.BaseUrl))
            {
                _logger.LogWarning(
                    "OrderService:UseHttpValidation bật nhưng BaseUrl trống — từ chối tạo feedback. Cấu hình BaseUrl hoặc tắt UseHttpValidation.");
                return false;
            }

            var path = o.EligibilityRelativePath.Trim().TrimStart('/');
            var baseUrl = o.BaseUrl.Trim().TrimEnd('/');
            var relative = $"{baseUrl}/{path}";
            var url = QueryHelpers.AddQueryString(relative, new Dictionary<string, string?>
            {
                ["customerId"] = customerId.ToString(),
                ["orderId"] = orderId.ToString(),
                ["productId"] = productId.ToString()
            });

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (!string.IsNullOrWhiteSpace(o.ApiKeyHeaderName) && !string.IsNullOrWhiteSpace(o.ApiKey))
                request.Headers.TryAddWithoutValidation(o.ApiKeyHeaderName, o.ApiKey);

            var client = _httpClientFactory.CreateClient(HttpClientName);
            client.Timeout = TimeSpan.FromSeconds(Math.Clamp(o.TimeoutSeconds, 1, 120));

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gọi order-service eligibility {Url}", url);
                return false;
            }

            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Forbidden)
                return false;

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                _logger.LogWarning(
                    "order-service trả 400 cho eligibility customerId={CustomerId} orderId={OrderId} productId={ProductId}",
                    customerId, orderId, productId);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "order-service trả {StatusCode} cho eligibility URL {Url}",
                    (int)response.StatusCode,
                    url);
                return false;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(body))
                return true;

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                foreach (var prop in root.EnumerateObject())
                {
                    if (!prop.Name.Equals("eligible", StringComparison.OrdinalIgnoreCase)
                        && !prop.Name.Equals("canReview", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (prop.Value.ValueKind == JsonValueKind.False)
                        return false;
                    if (prop.Value.ValueKind == JsonValueKind.True)
                        return true;
                }

                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Body eligibility không phải JSON hợp lệ; HTTP 200 nên coi là đủ điều kiện.");
                return true;
            }
        }
    }
}
