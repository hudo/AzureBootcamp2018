using System;
using System.Threading.Tasks;
using BootShop.Common;
using BootShop.Web.API.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BootShop.Web.API.Services
{
    public class MailerClient
    {
        private readonly IConfiguration _config;
        private readonly WebshopDbContext _db;

        public MailerClient(IConfiguration config, WebshopDbContext db)
        {
            _config = config;
            _db = db;
        }


        public async Task SendEmailNotification(Order order)
        {
            var queue = await GetQueue();

            var orderReceivedMessage = new OrderReceivedMessage {OrderId = order.Id, Amount = order.Amount};

            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(orderReceivedMessage));

            try
            {
                await queue.AddMessageAsync(queueMessage);
            }
            catch (Exception)
            {
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