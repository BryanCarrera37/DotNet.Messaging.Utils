using BryanCM.Messaging.Utils.IntegrationTests.Fixtures.Keycloak;

namespace BryanCM.Messaging.Utils.IntegrationTests.Fixtures.RabbitMQ
{
    [CollectionDefinition(nameof(RabbitMqFixtureCollection))]
    public class RabbitMqFixtureCollection
        : ICollectionFixture<KeycloakFixture>,
        ICollectionFixture<RabbitMqFixture>
    {
    }
}
