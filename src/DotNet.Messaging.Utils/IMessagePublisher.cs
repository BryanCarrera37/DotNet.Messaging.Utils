using BryanCM.AspNet.Utils.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DotNet.Messaging.Utils
{
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publish a message to the Parking Lot Configured in the Main Queue. Also, ack the message from the MainQueue.
        /// </summary>
        /// <param name="channel"><see cref="{IChannel}"/> that will be used to Ack the Message from the Main Queue.</param>
        /// <param name="args"><see cref="BasicDeliverEventArgs"/> related with the message consumed.</param>
        /// <param name="cancellationToken">Notification that specifies if the operation must be canceled.</param>
        /// <returns><see cref="Result"/> Object with the operation's outcome.</returns>
        Task<Result> RedirectMessageToParkingLotAsync(IChannel channel, BasicDeliverEventArgs args, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes a message to the queue using the Exchange and Routing Key specified.
        /// </summary>
        /// <param name="exchange">Name of the Exchange.</param>
        /// <param name="routingKey">Routing Key configured with the Exchange.</param>
        /// <param name="body">Body to be published.</param>
        /// <param name="cancellationToken">Notification that specifies if the operation must be canceled.</param>
        /// <returns><see cref="Result"/> Object with the operation's outcome.</returns>
        Task<Result> PublishMessageAsync(string exchange, string routingKey, byte[] body, CancellationToken cancellationToken = default);
    }
}
