using BryanCM.AspNet.Utils.Abstractions;

namespace DotNet.Messaging.Utils.Errors
{
    public sealed class MessagingErrors : BaseErrors
    {
        private static readonly string _scope = "MessageBroker";

        public static readonly Error NotEnqueued = GetError(_scope, nameof(NotEnqueued), "Message could not be enqueued");
        public static readonly Error QueueNotFound = GetError(_scope, nameof(QueueNotFound), "Queue not found or not configured");
        public static readonly Error ParkingLotExchangeNotConfigured = GetError(_scope, nameof(ParkingLotExchangeNotConfigured), "Parking Lot Exchange not configured.");
        public static readonly Error ParkingLotQueueNotConfigured = GetError(_scope, nameof(ParkingLotQueueNotConfigured), "Parking Lot Queue not configured.");
    }
}
