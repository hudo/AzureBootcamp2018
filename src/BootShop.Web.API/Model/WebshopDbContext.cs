using Microsoft.EntityFrameworkCore;

namespace BootShop.Web.API.Model
{
    public class WebshopDbContext : DbContext
    {
        public WebshopDbContext(DbContextOptions<WebshopDbContext> options) : base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
    }
}
