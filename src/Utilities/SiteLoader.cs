using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Aether.Models;

namespace Aether.Utilities
{
    public class SiteLoader
    {
        private static readonly JsonSerializerOptions _jsonOptions;

        static SiteLoader()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true)
                }
            };
        }

        public IEnumerable<Definition> Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Config file path must be provided", nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("sites.json not found", path);

            var json = File.ReadAllText(path);

            var config = JsonSerializer.Deserialize<SiteConfig>(json, _jsonOptions);

            if (config?.Sites == null)
                throw new InvalidOperationException("Failed to parse site definitions from config.");

            return config.Sites;
        }
    }
}