using System;
using System.Threading.Tasks;
using BootShop.Common;

namespace BootShop.Service.EmailSender
{
    public interface ISendGridClient
    {
        Task<bool> SendEmail(OrderReceivedMessage orderMessage);
    }

    public class ChaosMonkeySendGridClient : ISendGridClient
    {
        public Task<bool> SendEmail(OrderReceivedMessage orderMessage)
        {
            var random = new Random();
            var x = random.Next(10);

            if (x > 6)
                return Task.FromResult(false);
            else
                return Task.FromResult(true);
        }
    }
}