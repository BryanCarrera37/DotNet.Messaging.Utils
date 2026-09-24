using BryanCM.Messaging.Utils.Enums;
using BryanCM.Messaging.Utils.Errors;
using BryanCM.Messaging.Utils.Models;
using BryanCM.Messaging.Utils.Options;
using BryanCM.AspNet.Utils.Abstractions;
using RabbitMQ.Client;

namespace BryanCM.Messaging.Utils.Helpers
{
    public static class MessagingResourcesHelper
    {
        public static Exchange? GetExchangeByType(MessagingOptions options, ResourceType type)
            => options.Resources.Exchanges.FirstOrDefault(e => e.ResourceType == type);

        public static Result<BaseQueue> GetParkingLotFromRoutingKeyOfMainQueue(MessagingOptions options, string routingKeyOfMainQueue)
        {
            var possibleQueue = GetQueueOrDefault(options, routingKeyOfMainQueue);
            if (possibleQueue is null)
                return Result<BaseQueue>.Failure(MessagingErrors.QueueNotFound);

            return possibleQueue.ParkingLot is not null
                ? Result<BaseQueue>.Success(possibleQueue.ParkingLot)
                : Result<BaseQueue>.Failure(MessagingErrors.ParkingLotQueueNotConfigured);
        }

        public static Queue? GetQueueOrDefault(MessagingOptions options, string routingKey)
            => options.Resources.Queues!.FirstOrDefault(q => q.RoutingKey == routingKey || q.Name == routingKey);

        public static async Task DeclareResourcesAsync(IChannel channel, MessagingOptions options, CancellationToken cancellationToken)
        {
            await DeclareExchangesAsync(channel, options.Resources.Exchanges, cancellationToken);
            foreach (var queue in options.Resources.Queues)
            {
                if (queue.DeadLetter is not null)
                    await DeclareDeadLetterQueuesAndBindingsAsync(channel, queue, cancellationToken);

                if (queue.ParkingLot is not null)
                    await DeclareParkingLotQueuesAndBindingsAsync(channel, queue.ParkingLot, cancellationToken);

                await DeclareMainQueueAsync(channel, queue, cancellationToken);
            }
        }

        private static async Task DeclareExchangesAsync(IChannel channel, List<Exchange> exchanges, CancellationToken cancellationToken)
        {
            foreach (var ex in exchanges)
            {
                await channel.ExchangeDeclareAsync(
                    exchange: ex.Name,
                    type: ex.Type,
                    durable: true,
                    cancellationToken: cancellationToken);
            }
        }

        private static async Task DeclareDeadLetterQueuesAndBindingsAsync(
            IChannel channel,
            Queue queue,
            CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(
                    queue: queue.DeadLetter!.Name,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken,
                    arguments: new Dictionary<string, object?>
                    {
                        { "x-dead-letter-exchange", queue.ExchangeName },
                        { "x-dead-letter-routing-key", queue.RoutingKey },
                        { "x-message-ttl", queue.DeadLetter.TtlInSeconds * 1000 }
                    });

            await channel.QueueBindAsync(
                queue: queue.DeadLetter.Name,
                exchange: queue.DeadLetter.ExchangeName,
                routingKey: queue.DeadLetter.RoutingKey,
                cancellationToken: cancellationToken);
        }

        private static async Task DeclareParkingLotQueuesAndBindingsAsync(
            IChannel channel,
            BaseQueue parkingLotQueue,
            CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(
                queue: parkingLotQueue.Name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken);

            await channel.QueueBindAsync(
                queue: parkingLotQueue.Name,
                exchange: parkingLotQueue.ExchangeName,
                routingKey: parkingLotQueue.RoutingKey,
                cancellationToken: cancellationToken);
        }

        private static async Task DeclareMainQueueAsync(
            IChannel channel,
            Queue queue,
            CancellationToken cancellationToken)
        {
            await channel.QueueDeclareAsync(
                    queue: queue.Name,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken,
                    arguments: queue.DeadLetter is not null
                        ? new Dictionary<string, object?>
                        {
                            { "x-dead-letter-exchange", queue.DeadLetter.ExchangeName }
                        }
                        : null);

            await channel.QueueBindAsync(
                queue: queue.Name,
                exchange: queue.ExchangeName,
                routingKey: queue.RoutingKey,
                cancellationToken: cancellationToken);
        }
    }
}
