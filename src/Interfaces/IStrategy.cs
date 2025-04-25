using Aether.Models;

namespace Aether.Interfaces
{
    public interface IStrategy
    {
        Task<Result> CheckAsync(HttpClient http, Definition def, string identifier);
    }
}