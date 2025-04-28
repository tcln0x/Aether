namespace Aether.Models
{
    public class Result
    {
        public required string SiteKey { get; set; }

        public required string SiteName { get; set; }

        public required bool Exists { get; set; }

        public required string Url { get; set; }

        public Dictionary<string, string> Metadata { get; set; } = [];
    }
}