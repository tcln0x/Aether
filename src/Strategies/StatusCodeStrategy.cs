using Aether.Interfaces;
using Aether.Models;

namespace Aether.Strategies
{
    public class StatusCodeStrategy : IStrategy
    {
        public async Task<Result> CheckAsync(HttpClient http, Definition def, string identifier)
        {
            var url = def.UrlTemplate.Replace("{identifier}", identifier);

            try
            {
                if (def.DelayMs > 0)
                    await Task.Delay(def.DelayMs);

                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                foreach (var h in def.Headers)
                    req.Headers.TryAddWithoutValidation(h.Key, h.Value);

                using var response = await http.SendAsync(req);
                int code = (int)response.StatusCode;

                bool isPositive = def.PositiveStatus.Contains(code);
                bool isNegative = def.NegativeStatus.Contains(code);
                bool exists = isPositive && !isNegative;

                var result = new Result
                {
                    SiteKey = def.Key,
                    SiteName = def.Name,
                    Exists = exists,
                    Url = url
                };
                result.Metadata["Status"] = code.ToString();
                return result;
            }
            catch (HttpRequestException ex)
            {
                return ErrorResult(def, url, ex.Message);
            }
            catch (TaskCanceledException)
            {
                return ErrorResult(def, url, "Request timed out");
            }
            catch (Exception ex)
            {
                return ErrorResult(def, url, ex.Message);
            }
        }

        private static Result ErrorResult(Definition def, string url, string message)
        {
            var r = new Result
            {
                SiteKey = def.Key,
                SiteName = def.Name,
                Exists = false,
                Url = url
            };
            r.Metadata["Error"] = message;
            return r;
        }
    }
}