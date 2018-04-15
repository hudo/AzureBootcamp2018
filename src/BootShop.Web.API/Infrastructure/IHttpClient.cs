using System.Net.Http;
using System.Threading.Tasks;

namespace BootShop.Web.API.Infrastructure
{
    public interface IHttpClient
    {
        Task<T> GetAsync<T>(string uri);
        Task<HttpResponseMessage> PostAsync<T>(string uri, T item);
    }
}