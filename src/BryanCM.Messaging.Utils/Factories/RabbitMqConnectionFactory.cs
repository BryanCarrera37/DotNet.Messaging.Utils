using BryanCM.Messaging.Utils.Providers;
using RabbitMQ.Client;

namespace BryanCM.Messaging.Utils.Factories
{
    public class RabbitMqConnectionFactory(
        ITokenProvider tokenProvider,
        ConnectionFactory factory) : IConnectionFactorySsoSecured
    {
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly ConnectionFactory _factory = factory;

        public async Task<IConnection> CreateConnectionAsync(string clientProvidedName = "UNKNOWN", CancellationToken cancellationToken = default)
        {
            var jwt = await _tokenProvider.GetTokenAsync(cancellationToken);
            _factory.Password = jwt;
            _factory.ClientProvidedName = clientProvidedName;

            return await _factory.CreateConnectionAsync(cancellationToken);
        }
    }
}
