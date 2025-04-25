using System.Net;

namespace Aether.Factories
{
    public static class HttpClientFactory
    {
        public static HttpClient Create()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(15)
            };

            return client;
        }
    }
}
