namespace BryanCM.Messaging.Utils.Models
{
    public class Queue : BaseQueue
    {
        public DeadLetterQueue? DeadLetter { get; init; }
        public BaseQueue? ParkingLot { get; init; }
    }
}
