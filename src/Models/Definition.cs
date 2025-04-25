using Aether.Enums;

namespace Aether.Models
{
    public class Definition
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public string Template { get; set; }

        public CheckStrategy Strategy { get; set; }

        public List<int> MatchedCodes { get; set; } = [];

        public List<int> UnmatchedCodes { get; set; } = [];

        public List<string> MatchedStrings { get; set; } = [];

        public List<string> UnmatchedStrings { get; set; } = [];

        public List<string> Tags { get; set; } = [];
    }
}