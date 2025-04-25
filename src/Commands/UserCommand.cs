using Aether.Factories;
using Aether.Formatters;
using Aether.Models;
using Aether.Utilities;
using McMaster.Extensions.CommandLineUtils;

namespace Aether.Commands
{
    public static class UserCommand
    {
        public static void Configure(CommandLineApplication app)
        {
            var cmd = app.Command("user", command =>
            {
                /* Shortened alias */
                command.AddName("u");

                command.Description = "Check an username across multiple sites.";

                var identifierArg = command.Argument("identifier", "The username to check.").IsRequired();

                command.OnExecuteAsync(async (cancellationToken) =>
                {
                    IEnumerable<Definition> definitions;

                    try
                    {
                        definitions = SiteLoader.Load("Config/Sites.json");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error loading config: {ex.Message}");
                        return 1;
                    }

                    using var http = HttpClientFactory.Create();
                    var strategies = StrategyFactory.CreateDefaultStrategies();

                    var total = definitions.Count();
                    var results = new List<Result>();

                    await foreach (var result in AsyncExecutor.ExecuteAsync(definitions, identifierArg.Value, strategies, http, maxConcurrency: 50))
                    {
                        results.Add(result);
                    }

                    SpectreFormatter.Format(results);
                    return 0;
                });
            });
        }
    }
}