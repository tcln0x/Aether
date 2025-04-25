namespace Aether.Models
{
    public class Result
    {
        public string SiteKey { get; set; }

        public string SiteName { get; set; }

        public bool Exists { get; set; }

        public string Url { get; set; }

        public Dictionary<string, string> Metadata { get; set; } = [];
    }
}