using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Spectre.Console;
using Aether.Models;
using Aether.Utilities;
using Aether.Formatters;
using Aether.Strategies;
using Aether.Factories;

namespace Aether.Commands
{
    public static class UserCommand
    {
        public static void Configure(CommandLineApplication app)
        {
            app.Command("user", command =>
            {
                command.AddName("u");
                command.AddName("username");
                command.Description = "Check a username (or multiple via --file) across configured sites.";

                var identifierArg = command.Argument("identifier", "Identifier to check");
                var fileOption = command.Option(
                    "-f|--file <FILE>",
                    "Path to newline-separated list of identifiers",
                    CommandOptionType.SingleValue);

                var parallelOption = command.Option(
                    "-p|--parallelism <N>",
                    "Max concurrent requests per identifier (default 50)",
                    CommandOptionType.SingleValue);

                var allOption = command.Option(
                    "-a|--all",
                    "Show non-found results (default hides)",
                    CommandOptionType.NoValue);

                command.OnExecuteAsync(async ct =>
                {
                    string cfg = Path.Combine(AppContext.BaseDirectory, "Config", "Sites.json");
                    IEnumerable<Definition> defs;

                    var loader = new SiteLoader();
                    try
                    {
                        defs = loader.Load(Path.Join("Config", "Sites.json"));
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"Error loading config: {ex.Message}");
                        return 1;
                    }

                    List<string> ids;
                    if (fileOption.HasValue())
                    {
                        var path = fileOption.Value();
                        if (!File.Exists(path)) { Console.Error.WriteLine($"File not found: {path}"); return 1; }
                        ids = File.ReadLines(path).Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
                        if (!ids.Any()) { Console.Error.WriteLine("No identifiers in file."); return 1; }
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(identifierArg.Value))
                        {
                            Console.Error.WriteLine("Provide <identifier> or use --file."); return 1;
                        }
                        ids = new List<string> { identifierArg.Value };
                    }

                    int max = 50;
                    if (parallelOption.HasValue() && int.TryParse(parallelOption.Value(), out var px) && px > 0) max = px;

                    using var http = HttpClientFactory.Create();
                    var strategies = StrategyFactory.CreateDefaultStrategies();
                    bool showAll = allOption.HasValue();

                    var table = new Table().Border(TableBorder.Rounded)
                        .AddColumn("Identifier").AddColumn("Site").AddColumn("Found").AddColumn("URL");

                    int total = ids.Count * defs.Count();
                    int done = 0;

                    await AnsiConsole.Live(new Rows(table, new Markup("")))
                        .AutoClear(false)
                        .Overflow(VerticalOverflow.Visible)
                        .Cropping(VerticalOverflowCropping.Bottom)
                        .StartAsync(async ctx =>
                        {
                            var tasks = ids.Select(id => Task.Run(async () =>
                            {
                                await foreach (var res in AsyncExecutor.ExecuteAsync(defs, id, strategies, http, max))
                                {
                                    if (res.Exists || showAll)
                                    {
                                        table.AddRow(
                                            $"[yellow]{id}[/]",
                                            res.SiteName,
                                            res.Exists ? "[green]Yes[/]" : "[red]No[/]",
                                            $"[blue]{res.Url}[/]"
                                        );
                                    }
                                    done++;
                                    ctx.UpdateTarget(new Rows(
                                        table,
                                        new Markup($"[grey]Progress: {done}/{total}[/]")));
                                    ctx.Refresh();
                                }
                            }, ct)).ToList();

                            await Task.WhenAll(tasks);
                        });

                    return 0;
                });
            });
        }
    }
}
