using BootShop.Web.API.Model;
using Microsoft.AspNetCore.Mvc;

namespace BootShop.Web.API.Controllers
{
    [Route("api/health")]
    public class HealthCheckController : Controller
    {
        [ProducesResponseType(typeof(ServiceHealthModel), 200)]
        [ProducesResponseType(500)]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new[]
            {
                new ServiceHealthModel("payment", true),
                new ServiceHealthModel("mailer", true),
                new ServiceHealthModel("warehouse", true)
            });
        }
    }
}