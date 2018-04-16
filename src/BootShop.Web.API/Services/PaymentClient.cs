using System.Threading;
using System.Threading.Tasks;
using BootShop.Web.API.Infrastructure;
using BootShop.Web.API.Model;

namespace BootShop.Web.API.Services
{
    public class PaymentClient
    {
        private readonly IHttpClient _resilientHttpClient;

        public PaymentClient(IHttpClient resilientHttpClient)
        {
            _resilientHttpClient = resilientHttpClient;
        }

        public async Task Process(Order order)
        {
            order.Status = OrderStatus.PaymentPending;

            var response = await _resilientHttpClient.PostAsync("http://payment/api/order", order);

            if(!response.IsSuccessStatusCode)
                throw new PaymentServiceException();

            order.Status = OrderStatus.Payed;
        }
    }
}