namespace BootShop.Web.API.Infrastructure
{
    public interface IResilientHttpClientFactory
    {
        ResilientHttpClient CreateClient();
    }
}