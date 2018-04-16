using System;
using System.Threading.Tasks;
using BootShop.Web.API.Model;
using BootShop.Web.API.Services;

namespace BootShop.Web.API
{
    public class ProcessManager
    {
        private readonly WebshopDbContext _db;
        private readonly PaymentClient _paymentClient;
        private readonly MailerClient _mailerClient;

        public ProcessManager(
            WebshopDbContext db,
            PaymentClient paymentClient,
            MailerClient mailerClient)
        {
            _db = db;
            _paymentClient = paymentClient;
            _mailerClient = mailerClient;
        }

        public async Task<int> Handle(ProcessOrderCommand command)
        {
            Order order = null;

            using (var tx = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    order = CreateOrder(command.Amount);

                    await _db.SaveChangesAsync();

                    await _paymentClient.Process(order);

                    tx.Commit();
                }
                catch (PaymentServiceException)
                {
                    order.Status = OrderStatus.PaymentPending;

                    tx.Commit();

                    throw new Exception("Couldn't process the payment, please check your credit card provider and try again");
                }
                catch (Exception)
                {
                    tx.Rollback();

                    // log and bubble up

                    throw;
                }
            }

            await _mailerClient.SendEmailNotification(order);

            return order.Id;
        }


        private Order CreateOrder(decimal amount)
        {
            var order = new Order { Amount = amount, Status = OrderStatus.Created };

            _db.Orders.Add(order);

            return order;
        }
    }
}