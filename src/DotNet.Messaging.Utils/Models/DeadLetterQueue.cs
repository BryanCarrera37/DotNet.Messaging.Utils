namespace DotNet.Messaging.Utils.Models
{
    public class DeadLetterQueue : BaseQueue
    {
        public int TtlInSeconds { get; init; }
    }
}
