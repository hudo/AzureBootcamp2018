using System.Threading.Tasks;
using BootShop.Web.API.Model;

namespace BootShop.Web.API.Services
{
    public class PaymentService
    {
        public async Task Process(Order order)
        {
            order.Status = OrderStatus.PaymentPending;

            order.Status = OrderStatus.Payed;
        }
    }
}