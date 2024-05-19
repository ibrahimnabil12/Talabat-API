using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services.Interfaces;

namespace Talabat.APIs.Controllers
{
    [Authorize]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        const string endpointSecret = "whsec_a0e98ca4b7a1da4df8b4c09709f42538d817aa7d45f4820bc5bb884ee9afd746";

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket is null) return BadRequest(new ApiResponse(400, "there is a problem "));
             return Ok(basket);   
        }


        [AllowAnonymous]
        [HttpPost("webhook")] //post                   https://localhost:7219/api/payments/webhook
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);

                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                   await _paymentService.UpdatepaymentIntentToSuccessedOrFailed(paymentIntent.Id, false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatepaymentIntentToSuccessedOrFailed(paymentIntent.Id, true);
                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

    }
}
