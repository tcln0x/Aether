// src/Aether/Strategies/MessageStrategy.cs
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Aether.Interfaces;
using Aether.Models;

namespace Aether.Strategies
{
    public class MessageStrategy : IStrategy
    {
        public async Task<Result> CheckAsync(HttpClient http, Definition def, string identifier)
        {
            var url = def.UrlTemplate.Replace("{identifier}", identifier);

            if (def.DelayMs > 0)
                await Task.Delay(def.DelayMs);

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);
                foreach (var h in def.Headers)
                    req.Headers.TryAddWithoutValidation(h.Key, h.Value);

                using var res = await http.SendAsync(req);
                var content = await res.Content.ReadAsStringAsync();

                bool found = def.PresenceStrings.Any(p => content.Contains(p))
                          && !def.AbsenceStrings.Any(a => content.Contains(a));

                var result = new Result
                {
                    SiteKey = def.Key,
                    SiteName = def.Name,
                    Exists = found,
                    Url = url
                };
                result.Metadata["Status"] = ((int)res.StatusCode).ToString();
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