using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DotNet.Messaging.Utils.Services
{
    internal class RabbitMqConnectionManager(
        IConnectionFactorySsoSecured connectionFactory,
        ILogger<RabbitMqConnectionManager> logger) : IConnectionManager
    {
        private readonly IConnectionFactorySsoSecured _connectionFactory = connectionFactory;
        private readonly SemaphoreSlim _connectionLock = new(1, 1);
        private readonly ILogger _logger = logger;
        private IConnection? _connection;

        public bool IsConnected => _connection?.IsOpen == true;

        public async Task<IConnection> GetConnectionAsync(string clientProvidedName = "UNKNOWN", CancellationToken cancellationToken = default)
        {
            if (IsConnected)
                return _connection!;

            await _connectionLock.WaitAsync(cancellationToken);
            try
            {
                if (IsConnected)
                    return _connection!;

                _connection = await _connectionFactory.CreateConnectionAsync(clientProvidedName, cancellationToken);
                _logger.LogInformation("RabbitMQ Connection established successfully");
                return _connection;
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task DisposeAsync()
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }

        public async Task CloseConnectionAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConnected)
            {
                return;
            }

            await _connection!.CloseAsync(cancellationToken);
        }
    }
}
