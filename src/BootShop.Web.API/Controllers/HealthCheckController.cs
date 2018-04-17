using System;
using System.Threading.Tasks;
using BootShop.Web.API.Infrastructure;
using BootShop.Web.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BootShop.Web.API.Controllers
{
    [Route("api/health")]
    public class HealthCheckController : Controller
    {
        private readonly IHttpClient _httpClient;
        private readonly IConfiguration _config;

        public HealthCheckController(IHttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        [ProducesResponseType(typeof(ServiceHealthModel), 200)]
        [ProducesResponseType(500)]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var isPayOk = await CheckPayment();

            return Ok(new[]
            {
                new ServiceHealthModel("payment", isPayOk),
                new ServiceHealthModel("mailer", true),
            });
        }

        private async Task<bool> CheckPayment()
        {
            var payment = false;
            try
            {
                var domain = _config["PaymentService:uri"];
                var response = await _httpClient.GetAsync<string>($"http://{domain}/api/health");
                payment = true;
            }
            catch (Exception)
            {
                payment = false;
            }

            return payment;
        }
    }
}