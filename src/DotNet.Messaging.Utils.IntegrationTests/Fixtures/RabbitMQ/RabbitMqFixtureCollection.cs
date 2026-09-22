using DotNet.Messaging.Utils.IntegrationTests.Fixtures.Keycloak;

namespace DotNet.Messaging.Utils.IntegrationTests.Fixtures.RabbitMQ
{
    [CollectionDefinition(nameof(RabbitMqFixtureCollection))]
    public class RabbitMqFixtureCollection
        : ICollectionFixture<KeycloakFixture>,
        ICollectionFixture<RabbitMqFixture>
    {
    }
}
