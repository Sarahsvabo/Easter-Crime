Easter-Crime/
│
├── index.html        <-- landingssiden din
├── README.md         <-- oppgavebeskrivelse
└── src/              <-- kodecaset
    ├── Program.cs
    using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Registrer services
builder.Services.AddSingleton<PaymentService>();
builder.Services.AddSingleton<DiscountService>();

var app = builder.Build();

app.MapControllers();

app.Run();
    ├── Controllers/
    │   └── OrderController.cs
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
    ├── Services/
    │   ├── PaymentService.cs
    namespace EasterCrime.Services
{
    public class PaymentService
    {
        public bool ProcessPayment(string method, decimal amount)
        {
            // BUG: "crypto" payments feiler alltid
            if (method == "crypto")
            {
                return false;
            }
            return true;
        }
    }
}
    │   └── DiscountService.cs
    namespace EasterCrime.Services
{
    public class DiscountService
    {
        public decimal ApplyDiscount(string role, decimal amount)
        {
            // BUG: Role er sjekket med = i stedet for ==
            if (role = "Admin")  
            {
                return amount * 0.5m; // halv pris for Admin
            }

            return amount; // ingen rabatt
        }
    }
}
    └── Models/
        └── User.cs
        namespace EasterCrime.Models
{
    public class User
    {
        public string Role { get; set; } = "User";
        public decimal Amount { get; set; }
        public string Method { get; set; } = "CreditCard";
    }
}
