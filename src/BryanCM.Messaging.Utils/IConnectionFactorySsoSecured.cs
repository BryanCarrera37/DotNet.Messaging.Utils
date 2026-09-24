using RabbitMQ.Client;

namespace BryanCM.Messaging.Utils
{
    public interface IConnectionFactorySsoSecured
    {
        /// <summary>
        /// Create and return a new RabbitMQ connection using SSO secured authentication. The connection will be created with the provided client name, or "UNKNOWN" if no name is provided.
        /// </summary>
        /// <param name="clientProvidedName">Name that will be given to the connection so that monitors could be able to recognize which application is connected.</param>
        /// <param name="cancellationToken">CancellationToken.</param>
        /// <returns></returns>
        Task<IConnection> CreateConnectionAsync(string clientProvidedName = "UNKNOWN", CancellationToken cancellationToken = default);
    }
}
