using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;

namespace BootShop.Web.API.Infrastructure
{
    public class ResilientHttpClient : IHttpClient
    {
        private readonly Policy[] _policies;
        private readonly ILogger<ResilientHttpClient> _logger;
        private readonly HttpClient _httpClient;

        public ResilientHttpClient(Policy[] policies, ILogger<ResilientHttpClient> logger)
        {
            _policies = policies;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        private async Task<T> HttpInvoker<T>(string uri, Func<Task<T>> action)
        {
            var policyWrap = Policy.WrapAsync(_policies);
            var stopwatch = Stopwatch.StartNew();

            var response = await policyWrap.ExecuteAsync(action);

            stopwatch.Stop();
            _logger.LogDebug($"Invoked URI {uri} in {stopwatch.ElapsedMilliseconds}ms");

            return response;
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            return await HttpInvoker(uri, async () =>
            {
                var response = await _httpClient.GetAsync(new Uri(uri));

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                if (typeof(T) == typeof(string))
                {
                    object content = await response.Content.ReadAsStringAsync();
                    return (T)content;
                }

                var stream = await response.Content.ReadAsStreamAsync();
                
                using (var reader = new StreamReader(stream))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<T>(jsonReader);
                }
            });
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string uri, T item)
        {
            return await HttpInvoker(uri, async () =>
            {
                var response =  await _httpClient.PostAsync(new Uri(uri),
                    new StringContent(JsonConvert.SerializeObject(item), System.Text.Encoding.UTF8, "application/json"));

                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }

                return response;
            });
        }
    }
}