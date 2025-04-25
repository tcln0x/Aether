using Aether.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aether.Utilities
{
    public static class SiteLoader
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public static IEnumerable<Definition> Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Config file path must be provided", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("Sites.json not found", path);

            var json = File.ReadAllText(path);
            var config = JsonSerializer.Deserialize<SiteConfig>(json, _jsonOptions);

            if (config?.Sites == null)
                throw new InvalidOperationException("Failed to parse site definitions from config.");

            return config.Sites;
        }
    }
}
