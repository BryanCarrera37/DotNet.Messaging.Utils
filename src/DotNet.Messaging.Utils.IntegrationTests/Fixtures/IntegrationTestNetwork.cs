using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;

namespace DotNet.Messaging.Utils.IntegrationTests.Fixtures
{
    internal static class IntegrationTestNetwork
    {
        public static readonly string NetworkName = "messaging-utils-test-network";
        public static readonly INetwork Network = new NetworkBuilder()
            .WithName(NetworkName)
            .Build();
    }
}
