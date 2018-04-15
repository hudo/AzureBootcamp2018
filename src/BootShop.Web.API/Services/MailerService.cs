using System.Threading.Tasks;
using BootShop.Web.API.Model;

namespace BootShop.Web.API.Services
{
    public class MailerService
    {
        public  Task SendEmailNotification(Order order)
        {
            return Task.CompletedTask;
        }
    }
}