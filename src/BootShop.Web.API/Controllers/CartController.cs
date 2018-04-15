using System;
using System.Threading.Tasks;
using BootShop.Common;
using BootShop.Web.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace BootShop.Web.API.Controllers
{
    [Route("api/cart")]
    public class CartController : Controller
    {
        private readonly ProcessManager _manager;

        public CartController(ProcessManager manager)
        {
            _manager = manager;
        }

        [ProducesResponseType(typeof(Response), 200)]
        [ProducesResponseType(typeof(Response), 500)]
        [HttpPost]
        public async Task<IActionResult> Post(ProcessOrderCommand command)
        {
            try
            {
                var orderId = await _manager.Handle(command);

                return Ok(new Response
                {
                    IsSucessful = orderId > 0,
                    Message = $"Order created with ID {orderId}"
                });
            }
            catch (Exception e)
            {
                return StatusCode(500, new Response { IsSucessful = false, Message = e.Message });
            }
        }
    }
}
