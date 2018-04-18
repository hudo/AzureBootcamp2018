using System;
using System.Threading.Tasks;
using BootShop.Common;
using BootShop.Web.API.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BootShop.Web.API.Services
{
    public class MailerClient
    {
        private readonly IConfiguration _config;
        private readonly WebshopDbContext _db;
        private readonly ILogger<MailerClient> _logger;

        public MailerClient(IConfiguration config, WebshopDbContext db, ILogger<MailerClient> logger)
        {
            _config = config;
            _db = db;
            _logger = logger;
        }


        public async Task SendEmailNotification(Order order)
        {
            var queue = await GetQueue();

            var orderReceivedMessage = new OrderReceivedMessage {OrderId = order.Id, Amount = order.Amount};

            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(orderReceivedMessage));

            try
            {
                _logger.LogInformation("Sending email request to MailerService");

                await queue.AddMessageAsync(queueMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Couldn't send email request, saving to outbox for later retry");

                // save to outbox so we can try sending later!

                _db.Outbox.Add(new PendingOrderNotification {OrderId = order.Id});

                await _db.SaveChangesAsync();
            }
        }

        private async Task<CloudQueue> GetQueue()
        {
            var account = CloudStorageAccount.Parse(_config.GetConnectionString("StorageAccount"));
            var queueClient = account.CreateCloudQueueClient();
            var reference = queueClient.GetQueueReference(_config["MailerService:queue"]);

            await reference.CreateIfNotExistsAsync();

            return reference;
        }
    }
}