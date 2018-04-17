using System.Threading;
using System.Threading.Tasks;
using BootShop.Common;
using BootShop.Web.API.Infrastructure;
using BootShop.Web.API.Model;
using Microsoft.Extensions.Configuration;

namespace BootShop.Web.API.Services
{
    public class PaymentClient
    {
        private readonly IHttpClient _resilientHttpClient;
        private readonly IConfiguration _config;

        public PaymentClient(IHttpClient resilientHttpClient, IConfiguration config)
        {
            _resilientHttpClient = resilientHttpClient;
            _config = config;
        }

        public async Task Process(Order order)
        {
            order.Status = OrderStatus.PaymentPending;

            var serviceUrl = _config["PaymentService:uri"]; 

            var response = await _resilientHttpClient.PostAsync(
                $"http://{serviceUrl}/api/order",
                new OrderDto { OrderId = order.Id, Amount = order.Amount });

            if(!response.IsSuccessStatusCode)
                throw new PaymentServiceException();

            order.Status = OrderStatus.Payed;
        }
    }
}