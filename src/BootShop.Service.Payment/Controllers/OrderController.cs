using System;
using BootShop.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BootShop.Service.Payment.Controllers
{
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [Route(""), HttpPost]
        public IActionResult Post(OrderDto order)
        {
            var random = new Random();

            if (random.Next(10) > 6)
            {
                _logger.LogWarning("Stripe is down:(");

                return StatusCode(500, "Couldn't process the payment");
            }

            _logger.LogInformation("Payment was successfull, cha ching!");

            return Ok(order);
        }
    }
}