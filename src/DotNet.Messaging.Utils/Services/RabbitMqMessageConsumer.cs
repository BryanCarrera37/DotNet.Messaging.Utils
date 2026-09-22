using DotNet.Messaging.Utils.Helpers;
using DotNet.Messaging.Utils.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DotNet.Messaging.Utils.Services
{
    public class RabbitMqMessageConsumer(
        IConnectionManager connectionManager,
        ILogger logger,
        IOptions<MessagingOptions> messagingOptions)
    {
        protected readonly IConnectionManager connectionManager = connectionManager;
        protected readonly ILogger logger = logger;
        protected readonly MessagingOptions messagingOptions = messagingOptions.Value;

        protected IChannel? channel;

        public bool HasReachedTheRetryCountLimit(BasicDeliverEventArgs args)
        {
            if (args.BasicProperties.Headers is null || !args.BasicProperties.Headers.TryGetValue("x-death", out var deathValues) || deathValues is null)
                return false;

            var lastDeath = (Dictionary<string, object>)((List<object>)deathValues)[0];
            return Convert.ToInt32(lastDeath["count"]) > messagingOptions.RetryCountLimit;
        }

        public virtual async Task CloseAsync(CancellationToken cancellationToken = default)
        {
            if (channel is not null)
                await channel.CloseAsync(cancellationToken);

            if (connectionManager.IsConnected)
                await connectionManager.CloseConnectionAsync();
        }

        public virtual void DisposeElements()
        {
            channel?.Dispose();
            connectionManager.DisposeAsync();
        }

        public virtual async Task DeclareResourcesAsync(string clientProvidedName = "UNKNOWN", CancellationToken cancellationToken = default)
        {
            if (channel is null)
                await InitializeChannelAsync(clientProvidedName, cancellationToken);

            await MessagingResourcesHelper.DeclareResourcesAsync(channel!, messagingOptions, cancellationToken);
            logger.LogInformation("RabbitMQ Resources declared successfully");
        }

        public virtual async Task InitializeChannelAsync(string clientProvidedName = "UNKNOWN", CancellationToken cancellationToken = default)
        {
            var connection = await connectionManager.GetConnectionAsync(clientProvidedName, cancellationToken);
            channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
        }

        public virtual async Task StartConsumerInAllResourcesAsync(
            IChannel channel,
            AsyncEventingBasicConsumer consumer,
            bool autoAck = false,
            CancellationToken cancellationToken = default)
        {
            messagingOptions.Resources.Queues!.ForEach(async queue =>
            {
                await channel.BasicConsumeAsync(
                    queue: queue.Name,
                    autoAck: autoAck,
                    consumer: consumer,
                    cancellationToken: cancellationToken);
            });
        }
    }
}
