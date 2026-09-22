using DotNet.Messaging.Utils.IntegrationTests.Fixtures.Keycloak;
using DotNet.Messaging.Utils.IntegrationTests.Fixtures.RabbitMQ;
using DotNet.Messaging.Utils.IntegrationTests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Messaging.Utils.IntegrationTests.RabbitMQ
{
    [Collection(nameof(RabbitMqFixtureCollection))]
    public class RabbitMqConnectionHandleTests
    {
        private readonly KeycloakFixture _keycloakFixture;
        private readonly RabbitMqFixture _rabbitMqFixture;
        private readonly ServiceCollection _services;
        private readonly IConfiguration _configuration;

        public RabbitMqConnectionHandleTests(
            KeycloakFixture keycloakFixture,
            RabbitMqFixture rabbitMqFixture)
        {
            _keycloakFixture = keycloakFixture;
            _rabbitMqFixture = rabbitMqFixture;
            _services = InMemorySettingsHelper.CreateServiceCollection(
                _keycloakFixture.Authority,
                _rabbitMqFixture.Container.GetMappedPublicPort(RabbitMqFixture.Port),
                out var config);
            _configuration = config;
            _services.ConfigureRabbitMQ(_configuration, IdentityProvider.Keycloak);
        }

        [Fact]
        public async Task ConnectionManager_ShouldCreateOnlyOneConnection()
        {
            var connectionManager = _services.BuildServiceProvider().GetRequiredService<IConnectionManager>();
            Assert.NotNull(connectionManager);
            Assert.False(connectionManager.IsConnected);

            var connection = await connectionManager.GetConnectionAsync();
            Assert.NotNull(connection);
            Assert.True(connectionManager.IsConnected);

            var secondConnection = await connectionManager.GetConnectionAsync();
            Assert.NotNull(secondConnection);
            Assert.Same(connection, secondConnection);
        }
    }
}
