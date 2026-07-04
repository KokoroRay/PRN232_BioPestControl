using Microsoft.AspNetCore.Mvc;
using PayOS;
using PayOS.Types;
using System.Text.Json;

namespace payment_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PayOSClient _payOsClient;

        public PaymentController(PayOSClient payOsClient)
        {
            _payOsClient = payOsClient;
        }

        [HttpPost("create-payment-link")]
        public async Task<IActionResult> CreatePaymentLink([FromBody] PaymentRequest request)
        {
            try
            {
                var domain = "http://localhost:5173"; // Replace with your actual frontend domain
                var paymentData = new PaymentData(
                    orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                    amount: request.Amount,
                    description: $"Thanh toán đơn hàng",
                    items: new List<ItemData>(), // Option to add items here
                    cancelUrl: $"{domain}/checkout/payment?status=cancel",
                    returnUrl: $"{domain}/checkout/payment?status=success"
                );

                var createPayment = await _payOsClient.createPaymentLink(paymentData);
                return Ok(new { Success = true, CheckoutUrl = createPayment.checkoutUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public IActionResult Webhook([FromBody] WebhookType request)
        {
            try
            {
                var webhookData = _payOsClient.verifyPaymentWebhookData(request);
                // Here we would typically update the OrderStatus via gRPC or message queue to ordering-service
                // For now we just return Ok to acknowledge PayOS
                return Ok(new { Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
    }

    public class PaymentRequest
    {
        public Guid OrderId { get; set; }
        public int Amount { get; set; }
    }
}
