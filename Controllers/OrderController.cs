using EasterCrime.Models;
using EasterCrime.Services;
using Microsoft.AspNetCore.Mvc;

namespace EasterCrime.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly DiscountService _discountService;

        public OrderController(PaymentService paymentService, DiscountService discountService)
        {
            _paymentService = paymentService;
            _discountService = discountService;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] User user)
        {
            var discounted = _discountService.ApplyDiscount(user.Role, user.Amount);
            var paid = _paymentService.ProcessPayment(user.Method, discounted);

            if (!paid)
            {
                return StatusCode(500, "Payment failed!");
            }

            return Ok(new { amount = discounted, status = "success" });
        }
    }
}