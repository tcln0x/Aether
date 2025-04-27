using Aether.Models;
using Spectre.Console;

namespace Aether.Formatters
{
    public static class SpectreFormatter
    {
        public static void Format(IEnumerable<Result> results)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .AddColumn("[u]Site[/]")
                .AddColumn("[u]Exists[/]")
                .AddColumn("[u]Url[/]")
                .AddColumn("[u]Metadata[/]");

            foreach (var r in results)
            {
                var existsText = r.Exists ? "[green]Yes[/]" : "[red]No[/]";
                var details = string.Join("\n", r.Metadata.Select(kv => $"{kv.Key}: {kv.Value}"));

                table.AddRow(r.SiteName, existsText, r.Url, details);
            }

            AnsiConsole.Write(table);
        }
    }
}