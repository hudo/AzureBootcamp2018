using System;
using System.Threading.Tasks;
using BootShop.Web.API.Model;
using BootShop.Web.API.Services;

namespace BootShop.Web.API
{
    public class ProcessManager
    {
        private readonly WebshopDbContext _db;
        private readonly PaymentService _paymentService;
        private readonly MailerService _mailerService;

        public ProcessManager(
            WebshopDbContext db,
            PaymentService paymentService,
            MailerService mailerService)
        {
            _db = db;
            _paymentService = paymentService;
            _mailerService = mailerService;
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

                    await _paymentService.Process(order);

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

                    throw;
                }
            }

            await _mailerService.SendEmailNotification(order);

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