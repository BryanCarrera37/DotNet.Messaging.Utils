
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace BryanCM.Messaging.Utils.IntegrationTests.Fixtures.Keycloak
{
    public class KeycloakFixture : IAsyncLifetime
    {
        private static readonly string _resourcesFolder = "./../../../Fixtures/Keycloak";
        private static readonly string _image = "quay.io/keycloak/keycloak:26.3.2";
        private static readonly string _keycloakConfFolder = "/opt/keycloak/conf";

        public static string ClientId => "rabbitmq-authenticator";
        public static string ClientSecret => "eGN4cVgyVERwTVJ6T2VnaDBEbGpUdWF6NnN2bFRpaXA=";
        public static string UserPassword => "aG9sYW11bmRvLg==";
        public static string Username => "username.rabbitmq";

        private readonly ushort _port = 8443;
        private readonly ushort _insecurePort = 8080;
        private string _authority = string.Empty;

        public IContainer Container { get; private set; }
        public string Authority => _authority;

        public KeycloakFixture()
        {
            Container = new ContainerBuilder(_image)
                .WithName("messaging-utils-keycloak")
                .WithResourceMapping(
                    $"{_resourcesFolder}/Import/import.json",
                    "/opt/keycloak/data/import")
                .WithResourceMapping($"{_resourcesFolder}/Certs", $"{_keycloakConfFolder}")
                .WithEnvironment("KC_BOOTSTRAP_ADMIN_USERNAME", "admin")
                .WithEnvironment("KC_BOOTSTRAP_ADMIN_PASSWORD", "admin")
                .WithEnvironment("KC_HTTP_ENABLED", "true")
                .WithEnvironment("KC_HTTPS_ENABLED", "true")
                .WithEnvironment("KC_HTTP_PORT", $"{_insecurePort}")
                .WithEnvironment("KC_HTTPS_PORT", $"{_port}")
                .WithEnvironment("KC_HTTPS_CERTIFICATE_FILE", $"{_keycloakConfFolder}/keycloak.crt")
                .WithEnvironment("KC_HTTPS_CERTIFICATE_KEY_FILE", $"{_keycloakConfFolder}/keycloak.key")
                .WithPortBinding(_port, true)
                .WithPortBinding(_insecurePort, true)
                .WithWaitStrategy(Wait.ForUnixContainer()
                    .UntilInternalTcpPortIsAvailable(_port)
                    .UntilInternalTcpPortIsAvailable(_insecurePort))
                .WithNetwork(IntegrationTestNetwork.Network)
                .WithNetworkAliases("messaging-utils-keycloak")
                .WithCommand("start-dev", "--import-realm")
                .Build();
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            await Container.StartAsync();
            _authority = $"http://localhost:{Container.GetMappedPublicPort(_insecurePort)}";
        }
    }
}
