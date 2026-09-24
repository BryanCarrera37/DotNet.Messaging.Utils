namespace BryanCM.Messaging.Utils.Models
{
    public class BaseQueue
    {
        public string Name { get; init; } = string.Empty;
        public string RoutingKey { get; init; } = string.Empty;
        public string ExchangeName { get; init; } = string.Empty;
    }
}
