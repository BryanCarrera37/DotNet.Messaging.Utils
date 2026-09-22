namespace DotNet.Messaging.Utils.Models
{
    public class MessagingResources
    {
        public string VHost { get; init; } = string.Empty;
        public List<Exchange> Exchanges { get; init; } = default!;
        public List<Queue> Queues { get; init; } = default!;
    }

    public class MessagingResources<TQueue>
    {
        public string VHost { get; init; } = string.Empty;
        public List<Exchange> Exchanges { get; init; } = default!;
        public List<TQueue> Queues { get; init; } = default!;
    }
}
