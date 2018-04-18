using System.Threading.Tasks;
using BootShop.Common;
using BootShop.Web.API.Infrastructure;
using BootShop.Web.API.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BootShop.Web.API.Services
{
    public class PaymentClient
    {
        private readonly IHttpClient _resilientHttpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentClient> _logger;

        public PaymentClient(IHttpClient resilientHttpClient, IConfiguration config, ILogger<PaymentClient> logger)
        {
            _resilientHttpClient = resilientHttpClient;
            _config = config;
            _logger = logger;
        }

        public async Task Process(Order order)
        {
            order.Status = OrderStatus.PaymentPending;

            var serviceUrl = _config["PaymentService:uri"]; 

            _logger.LogInformation("Sending payment request");

            var response = await _resilientHttpClient.PostAsync(
                $"http://{serviceUrl}/api/order",
                new OrderDto { OrderId = order.Id, Amount = order.Amount });

            if (!response.IsSuccessStatusCode)
            {
                var contant = await response.Content.ReadAsStringAsync();

                _logger.LogWarning($"Error sending payment request, status code was {response.StatusCode}, content: {contant}");

                throw new PaymentServiceException();
            }

            order.Status = OrderStatus.Payed;
        }
    }
}