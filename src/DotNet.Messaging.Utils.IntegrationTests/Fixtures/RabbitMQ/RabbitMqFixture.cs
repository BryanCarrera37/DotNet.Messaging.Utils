
using DotNet.Testcontainers.Containers;
using Testcontainers.RabbitMq;

namespace DotNet.Messaging.Utils.IntegrationTests.Fixtures.RabbitMQ
{
    public class RabbitMqFixture : IAsyncLifetime
    {
        private static readonly string _importResourcesFolder = "./../../../Fixtures/RabbitMQ/Import";
        private static readonly string _image = "rabbitmq:4.1.3-management-alpine";
        private static readonly string _idpUsername = "username.rabbitmq";
        private static readonly string _configFolder = "/etc/rabbitmq";

        public static ushort Port => 5672;

        public IContainer Container { get; private set; }

        public RabbitMqFixture()
        {
            Container = new RabbitMqBuilder(_image)
                .WithName("messaging-utils-rabbitmq")
                .WithPortBinding(Port, Port)
                .WithUsername(_idpUsername)
                .WithResourceMapping($"{_importResourcesFolder}/10-oauth.conf", $"{_configFolder}/conf.d")
                .WithResourceMapping($"{_importResourcesFolder}/enabled_plugins", $"{_configFolder}")
                .WithNetwork(IntegrationTestNetwork.Network)
                .WithNetworkAliases("messaging-utils-rabbitmq")
                .Build();
        }

        public async Task DisposeAsync()
        {
            await Container!.DisposeAsync();
        }

        public async Task InitializeAsync()
        {
            await Container.StartAsync();
        }
    }
}
