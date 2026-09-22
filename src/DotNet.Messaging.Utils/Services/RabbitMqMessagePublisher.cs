using DotNet.Messaging.Utils.Errors;
using DotNet.Messaging.Utils.Helpers;
using DotNet.Messaging.Utils.Options;
using BryanCM.AspNet.Utils.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DotNet.Messaging.Utils.Services
{
    public class RabbitMqMessagePublisher(
        IConnectionManager connectionManager,
        IOptions<MessagingOptions> messagingOptions,
        ILogger<RabbitMqMessagePublisher> logger) : IMessagePublisher
    {
        private readonly IConnectionManager _connectionManager = connectionManager;
        protected readonly MessagingOptions messagingOptions = messagingOptions.Value;
        private readonly ILogger<RabbitMqMessagePublisher> _logger = logger;

        public virtual async Task<Result> RedirectMessageToParkingLotAsync(
            IChannel channel,
            BasicDeliverEventArgs args,
            CancellationToken cancellationToken = default)
        {
            var parkingLotQueue = MessagingResourcesHelper.GetParkingLotFromRoutingKeyOfMainQueue(messagingOptions, args.RoutingKey);
            if (parkingLotQueue.IsFailure)
            {
                _logger.LogError("Parking Lot Resource not found (routingKey = {RT}).", args.RoutingKey);
                return Result.Failure(MessagingErrors.ParkingLotQueueNotConfigured);
            }

            if (string.IsNullOrEmpty(parkingLotQueue.Data!.ExchangeName))
            {
                _logger.LogError("Parking Lot Exchange not found (parkingLotQueue = {Queue})", parkingLotQueue.Data.Name);
                return Result.Failure(MessagingErrors.ParkingLotExchangeNotConfigured);
            }

            await PublishMessageAsync(parkingLotQueue.Data!.ExchangeName, parkingLotQueue.Data!.RoutingKey, args.Body.ToArray(), cancellationToken);
            await channel!.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
            return Result.Success();
        }

        public async Task<Result> PublishMessageAsync(string exchange, string routingKey, byte[] body, CancellationToken cancellationToken = default)
        {
            try
            {
                var conn = await _connectionManager.GetConnectionAsync(messagingOptions.ClientProvidedName, cancellationToken);
                using var channel = await conn.CreateChannelAsync(cancellationToken: cancellationToken);
                await channel.BasicPublishAsync(
                    exchange: exchange,
                    routingKey: routingKey,
                    body: body,
                    cancellationToken: cancellationToken);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when trying to publish message to RabbitMQ");
                return Result.Failure(MessagingErrors.NotEnqueued);
            }
        }
    }
}
