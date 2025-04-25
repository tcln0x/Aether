using Aether.Commands;
using McMaster.Extensions.CommandLineUtils;

namespace Aether
{
    static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var app = new CommandLineApplication()
            {
                FullName = "Aether",
                Name = "Aether",
                Description = "Trace anyone, anything, anywhere."
            };

            app.HelpOption(inherited: true);
            app.VersionOption("-v|--version", "0.1.0");

            app.OnExecute(() =>
            {
                Console.WriteLine("Run `Aether --help` for commands.");
                return 0;
            });

            /* Register commands */
            UserCommand.Configure(app);

            return await app.ExecuteAsync(args);
        }
    }
}
