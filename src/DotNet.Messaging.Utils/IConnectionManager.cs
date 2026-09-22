using RabbitMQ.Client;

namespace DotNet.Messaging.Utils
{
    public interface IConnectionManager
    {
        /// <summary>
        /// Creates an unique RabbitMQ Connection with the SSO configured.
        /// </summary>
        /// <param name="cancellationToken">Notification that specifies if the operation must be canceled.</param>
        /// <returns><see cref="IConnection"/> for the Identity Provider configured.</returns>
        Task<IConnection> GetConnectionAsync(string clientProvidedName = "UNKNOWN", CancellationToken cancellationToken = default);
        Task CloseConnectionAsync(CancellationToken cancellationToken = default);
        Task DisposeAsync();
        bool IsConnected { get; }
    }
}
