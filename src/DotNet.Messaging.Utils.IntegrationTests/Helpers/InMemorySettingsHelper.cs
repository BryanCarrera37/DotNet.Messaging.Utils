using DotNet.Messaging.Utils.IntegrationTests.Fixtures.Keycloak;
using DotNet.Messaging.Utils.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Messaging.Utils.IntegrationTests.Helpers
{
    public static class InMemorySettingsHelper
    {
        public static ServiceCollection CreateServiceCollection(
            string keycloakAuthority,
            int amqpPort,
            out IConfiguration configuration)
        {
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(GetOptions(keycloakAuthority, amqpPort)!)
                .Build();

            return new ServiceCollection();
        }

        private static Dictionary<string, string?> GetOptions(string keycloakAuthority, int amqpPort)
            => new()
            {
                { $"{MessagingOptions.SectionName}:PayloadEncryptionKey", "some-encryption-key" },
                { $"{MessagingOptions.SectionName}:RetryCountLimit", "5" },
                { $"{RabbitMqOptions.SectionName}:Host", "localhost" },
                { $"{RabbitMqOptions.SectionName}:Port", $"{amqpPort}" },
                { $"{RabbitMqOptions.SectionName}:VHost", "/" },
                { $"{KeycloakOptions.SectionName}:GrantType", "password" },
                { $"{KeycloakOptions.SectionName}:Scope", "openid" },
                { $"{KeycloakOptions.SectionName}:Authority", keycloakAuthority },
                { $"{KeycloakOptions.SectionName}:Realm", "development" },
                { $"{KeycloakOptions.SectionName}:ClientId", KeycloakFixture.ClientId },
                { $"{KeycloakOptions.SectionName}:ClientSecret", KeycloakFixture.ClientSecret },
                { $"{KeycloakOptions.SectionName}:Username", KeycloakFixture.Username },
                { $"{KeycloakOptions.SectionName}:Password", KeycloakFixture.UserPassword },
                { $"{KeycloakOptions.SectionName}:TimeoutSeconds", "30" },
            };
    }
}
