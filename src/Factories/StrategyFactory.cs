using Aether.Enums;
using Aether.Interfaces;
using Aether.Strategies;

namespace Aether.Factories
{
    public static class StrategyFactory
    {
        public static Dictionary<CheckStrategy, IStrategy> CreateDefaultStrategies()
        {
            return new Dictionary<CheckStrategy, IStrategy>
            {
                { CheckStrategy.StatusCode, new StatusCodeStrategy() },
                { CheckStrategy.Message,    new MessageStrategy()    }
            };
        }
    }
}