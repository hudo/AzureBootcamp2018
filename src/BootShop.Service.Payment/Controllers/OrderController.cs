using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BootShop.Common;
using Microsoft.AspNetCore.Mvc;

namespace BootShop.Service.Payment.Controllers
{
    [Route("api/order")]
    public class OrderController : Controller
    {
        public IActionResult Post(OrderDto order)
        {
            var random = new Random();

            if (random.Next(10) > 6)
                return StatusCode(500, "Couldn't process the payment");

            return Ok(order);
        }
    }
}