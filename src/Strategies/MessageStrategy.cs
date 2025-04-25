using Aether.Interfaces;
using Aether.Models;

namespace Aether.Strategies
{
    public class MessageStrategy : IStrategy
    {
        public async Task<Result> CheckAsync(HttpClient http, Definition def, string identifier)
        {
            var url = def.Template.Replace("{identifier}", identifier);

            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);

                var response = await http.SendAsync(req);

                var content = await response.Content.ReadAsStringAsync();

                bool exists = def.MatchedStrings.Any(p => content.Contains(p))
                          && !def.MatchedStrings.Any(a => content.Contains(a));

                return new Result
                {
                    SiteKey = def.Key,
                    SiteName = def.Name,
                    Exists = exists,
                    Url = url,
                    Metadata = { ["Status"] = ((int)response.StatusCode).ToString() }
                };
            }
            catch (HttpRequestException ex)
            {
                return new Result
                {
                    SiteKey = def.Key,
                    SiteName = def.Name,
                    Exists = false,
                    Url = url,
                    Metadata = { ["Error"] = ex.Message }
                };
            }
            catch (TaskCanceledException)
            {
                return new Result
                {
                    SiteKey = def.Key,
                    SiteName = def.Name,
                    Exists = false,
                    Url = url,
                    Metadata = { ["Error"] = "Request timed out" }
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    SiteKey = def.Key,
                    SiteName = def.Name,
                    Exists = false,
                    Url = url,
                    Metadata = { ["Error"] = ex.Message }
                };
            }
        }
    }
}