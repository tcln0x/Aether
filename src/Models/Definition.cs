using Aether.Enums;

namespace Aether.Models
{
    public class Definition
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string UrlTemplate { get; set; }

        public string? UrlMain { get; set; }

        public int? Rank { get; set; }

        public CheckStrategy Strategy { get; set; }

        public List<int> PositiveStatus { get; set; } = [];

        public List<int> NegativeStatus { get; set; } = [];

        public List<string> PresenceStrings { get; set; } = [];

        public List<string> AbsenceStrings { get; set; } = [];

        public Dictionary<string, string> Headers { get; set; } = [];

        public string? ExtractPattern { get; set; }

        public List<string> Tags { get; set; } = [];

        public int DelayMs { get; set; } = 0;

        public int Retries { get; set; } = 3;
    }
}