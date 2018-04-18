
using Microsoft.AspNetCore.Mvc;

namespace BootShop.Service.Payment.Controllers
{
    [Route("api/health")]
    public class HealthController : Controller
    {
        public IActionResult Get()
        {
            return Ok("UP");
        }
    }
}