using Aether.Enums;
using Aether.Interfaces;
using Aether.Models;

namespace Aether.Utilities
{
    public static class AsyncExecutor
    {
        public static async IAsyncEnumerable<Result> ExecuteAsync(IEnumerable<Definition> definitions, string identifier, Dictionary<CheckStrategy, IStrategy> strategies, HttpClient http, int maxConcurrency = 100)
        {
            var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
            var tasks = new List<Task<Result>>();

            foreach (var def in definitions)
            {
                await semaphore.WaitAsync();
                tasks.Add(ExecuteSingleAsync(def));
            }

            while (tasks.Any())
            {
                var finished = await Task.WhenAny(tasks);
                tasks.Remove(finished);
                yield return await finished;
            }

            async Task<Result> ExecuteSingleAsync(Definition def)
            {
                try
                {
                    return await strategies[def.Strategy].CheckAsync(http, def, identifier);
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }
    }
}