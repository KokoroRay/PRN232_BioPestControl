using Microsoft.AspNetCore.Mvc;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
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
                var paymentData = new CreatePaymentLinkRequest
                {
                    OrderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                    Amount = (long)request.Amount,
                    Description = "Thanh toán đơn hàng",
                    Items = new List<PaymentLinkItem>(), // Option to add items here
                    CancelUrl = $"{domain}/checkout/payment?status=cancel",
                    ReturnUrl = $"{domain}/checkout/payment?status=success"
                };

                var createPayment = await _payOsClient.PaymentRequests.CreateAsync(paymentData);
                return Ok(new { Success = true, CheckoutUrl = createPayment.CheckoutUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] Webhook request)
        {
            try
            {
                var webhookData = await _payOsClient.Webhooks.VerifyAsync(request);
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
        public decimal Amount { get; set; }
    }
}
