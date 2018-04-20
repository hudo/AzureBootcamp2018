using System;
using System.Threading.Tasks;
using BootShop.Web.API.Model;
using BootShop.Web.API.Services;
using Microsoft.Extensions.Logging;

namespace BootShop.Web.API
{
    public class ProcessManager
    {
        private readonly WebshopDbContext _db;
        private readonly PaymentClient _paymentClient;
        private readonly MailerClient _mailerClient;
        private readonly ILogger<ProcessManager> _logger;

        public ProcessManager(
            WebshopDbContext db,
            PaymentClient paymentClient,
            MailerClient mailerClient,
            ILogger<ProcessManager> logger)
        {
            _db = db;
            _paymentClient = paymentClient;
            _mailerClient = mailerClient;
            _logger = logger;
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

                    // req/resp model for payment service
                    // http or queues

                    await _paymentClient.Process(order);

                    tx.Commit();
                }
                catch (PaymentServiceException)
                {
                    order.Status = OrderStatus.PaymentPending;

                    tx.Commit();

                    throw new Exception("Couldn't process the payment, please check your credit card provider and try again");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error processing order {order?.Id}");

                    tx.Rollback();

                    // log and bubble up

                    throw;
                }
            }

            // fire and forget to downstream systems
            // pub/sub model

            await _mailerClient.SendEmailNotification(order);

            _logger.LogInformation($"Order {order.Id} processed successfully");

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