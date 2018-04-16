using System;
using System.Threading;
using System.Threading.Tasks;
using BootShop.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace BootShop.Service.EmailSender
{
    public class NewOrdersReceiverService : BackgroundService
    {
        private readonly IConfiguration _config;
        private readonly ISendGridClient _sendGridClient;
        private readonly ILogger<BackgroundService> _logger;

        public NewOrdersReceiverService(IConfiguration config, ISendGridClient sendGridClient, ILogger<BackgroundService> logger) : base(logger)
        {
            _config = config;
            _sendGridClient = sendGridClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var queue = await GetQueue();

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Checking for queue message");

                var message = await queue.GetMessageAsync(TimeSpan.FromMinutes(5), null, null, cancellationToken);

                if (message != null)
                {
                    _logger.LogInformation("Message received!");

                    await TrySendEmail(message, queue);
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }

        private async Task TrySendEmail(CloudQueueMessage message, CloudQueue queue)
        {
            var orderReceived = JsonConvert.DeserializeObject<OrderReceivedMessage>(message.AsString);

            var isSuccessful = await _sendGridClient.SendEmail(orderReceived);

            if (isSuccessful)
            {
                await queue.DeleteMessageAsync(message);
            }
            else
            {
                _logger.LogInformation($"Couldn't send email for order {orderReceived.OrderId}");

                if (message.DequeueCount > 5)
                {
                    _logger.LogInformation($"Sending message to DLQ");

                    await queue.DeleteMessageAsync(message);
                }
            }
        }

        private async Task<CloudQueue> GetQueue()
        {
            var account = CloudStorageAccount.Parse(_config["ConnectionStrings:StorageAccount"]);
            var queueClient = account.CreateCloudQueueClient();
            var reference = queueClient.GetQueueReference(_config["MailerService:queue"]);

            await reference.CreateIfNotExistsAsync();

            return reference;
        }
}
}