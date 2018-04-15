namespace BootShop.Common
{
    public class ProcessPaymentCommand
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}