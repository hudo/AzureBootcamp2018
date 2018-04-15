namespace BootShop.Web.API.Model
{
    public class Order
    {
        public Order()
        {
            Status = OrderStatus.Created;
        }

        public int Id { get; set; }
        public decimal Amount { get; set; }

        public OrderStatus Status { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        PaymentPending,
        Payed,
        Delivered
    }
}