using DotNet.Messaging.Utils.Models;
using DotNet.Messaging.Utils.Options;
using BryanCM.AspNet.Utils.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DotNet.Messaging.Utils.UnitTests
{
    public class DependencyInjectionTests
    {
        private static readonly string _clientId = Guid.NewGuid().ToString();
        private static readonly string _tenantId = Guid.NewGuid().ToString();

        private readonly Dictionary<string, string?> _messagingOptions = new()
        {
            { $"{MessagingOptions.SectionName}:PayloadEncryptionKey", "some-encryption-key" },
            { $"{MessagingOptions.SectionName}:RetryCountLimit", "5" },
            { $"{RabbitMqOptions.SectionName}:Host", "some.host" },
            { $"{RabbitMqOptions.SectionName}:Port", "5672" },
            { $"{RabbitMqOptions.SectionName}:VHost", "some-vhost/" },
            { $"{EntraIdOptions.SectionName}:ClientId", _clientId },
            { $"{EntraIdOptions.SectionName}:ClientSecret", "c29tZS1jbGllbnQtc2VjcmV0" },
            { $"{EntraIdOptions.SectionName}:Scope", $"api://{_clientId}/.default" },
            { $"{EntraIdOptions.SectionName}:Authority", $"https://login.microsoftonline.com/{_tenantId}/oauth2/v2.0" }
        };

        [Fact]
        public void ConfigureRabbitMQ_ShouldConfigureTheMessagingOptions()
        {
            var services = CreateServiceCollection(out var configuration);
            services.ConfigureRabbitMQ(configuration, IdentityProvider.EntraId);

            var settings = services.BuildServiceProvider()
                .GetRequiredService<IOptionsMonitor<MessagingOptions>>();

            Assert.NotNull(settings);
            Assert.NotNull(settings.CurrentValue);
            Assert.Equal("some-encryption-key", settings.CurrentValue.PayloadEncryptionKey);
            Assert.Equal(5, settings.CurrentValue.RetryCountLimit);
        }

        [Fact]
        public void ConfigureRabbitMQ_ShouldConfigureTheMessagingOAuthResources()
        {
            var services = CreateServiceCollection(out var configuration);
            services.ConfigureRabbitMQ(configuration, IdentityProvider.EntraId);

            var sp = services.BuildServiceProvider();
            var rabbitMqSettings = sp.GetRequiredService<IOptionsMonitor<RabbitMqOptions>>();
            var entraIdSettings = sp.GetRequiredService<IOptionsMonitor<EntraIdOptions>>();

            Assert.NotNull(rabbitMqSettings.CurrentValue);
            Assert.NotNull(rabbitMqSettings.CurrentValue.Host);
            Assert.NotNull(rabbitMqSettings.CurrentValue.Port);
            Assert.NotNull(rabbitMqSettings.CurrentValue.VHost);
            Assert.Equal("some.host", rabbitMqSettings.CurrentValue.Host);
            Assert.Equal(5672, rabbitMqSettings.CurrentValue.Port);
            Assert.Equal("some-vhost/", rabbitMqSettings.CurrentValue.VHost);

            Assert.NotNull(entraIdSettings.CurrentValue);
            Assert.NotNull(entraIdSettings.CurrentValue.ClientId);
            Assert.NotNull(entraIdSettings.CurrentValue.ClientSecret);
            Assert.NotNull(entraIdSettings.CurrentValue.Scope);
            Assert.NotNull(entraIdSettings.CurrentValue.Authority);
            Assert.Equal(_clientId, entraIdSettings.CurrentValue.ClientId);
            Assert.Contains(_tenantId, entraIdSettings.CurrentValue.Authority);
        }

        [Fact]
        public void ConfigureRabbitMQ_ShouldConfigureConnectionManager()
        {
            var services = CreateServiceCollection(out var configuration);
            services.ConfigureRabbitMQ(configuration, IdentityProvider.EntraId);

            var manager = services.BuildServiceProvider()
                .GetRequiredService<IConnectionManager>();

            Assert.NotNull(manager);
            Assert.Equal("RabbitMqConnectionManager", manager.GetType().Name);
        }

        [Fact]
        public void ConfigureRabbitMQ_ShouldConfigureDefaultPublisher()
        {
            var services = CreateServiceCollection(out var configuration);
            services.ConfigureRabbitMQ(configuration, IdentityProvider.EntraId);

            var publisher = services.BuildServiceProvider()
                .GetRequiredService<IMessagePublisher>();

            Assert.NotNull(publisher);
            Assert.Equal("RabbitMqMessagePublisher", publisher.GetType().Name);
            Assert.Equal("DotNet.Messaging.Utils.Services", publisher.GetType().Namespace);
            Assert.Contains("DotNet.Messaging.Utils", publisher.GetType().Assembly.FullName);
        }

        [Fact]
        public void ConfigureRabbitMQ_ShouldConfigureCustomPublisher()
        {
            var services = CreateServiceCollection(out var configuration);
            services.ConfigureRabbitMQ<CustomPublisher>(configuration, IdentityProvider.EntraId);

            var publisher = services.BuildServiceProvider()
                .GetRequiredService<IMessagePublisher>();

            Assert.NotNull(publisher);
            Assert.Equal("CustomPublisher", publisher.GetType().Name);
            Assert.Equal(typeof(CustomPublisher).Assembly, publisher.GetType().Assembly);
        }

        [Fact]
        public void ConfigureRabbitMQ_ShouldConfigureCustomPublisherAndCustomQueue()
        {
            var services = CreateServiceCollection(out var configuration);
            services.ConfigureRabbitMQ<CustomPublisher, CustomQueue>(configuration, IdentityProvider.EntraId);

            var sp = services.BuildServiceProvider();
            var publisher = sp.GetRequiredService<IMessagePublisher>();
            var messagingOptions = sp.GetRequiredService<IOptionsMonitor<MessagingOptions<CustomQueue>>>();
            var messagingOptionsWithDefaultType = sp.GetRequiredService<IOptionsMonitor<MessagingOptions>>();

            Assert.NotNull(publisher);
            Assert.NotNull(messagingOptions);
            Assert.Empty(messagingOptionsWithDefaultType.CurrentValue.PayloadEncryptionKey);
            Assert.Equal("CustomPublisher", publisher.GetType().Name);
            Assert.Equal(typeof(CustomPublisher).Assembly, publisher.GetType().Assembly);
        }

        private ServiceCollection CreateServiceCollection(out IConfiguration configuration)
        {
            configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_messagingOptions!)
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();
            return services;
        }
    }

    internal class CustomPublisher : IMessagePublisher
    {
        public Task<Result> PublishMessageAsync(string exchange, string routingKey, byte[] body, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Result> RedirectMessageToParkingLotAsync(IChannel channel, BasicDeliverEventArgs args, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    internal class CustomQueue : Queue
    {
        public string EventKey { get; set; } = string.Empty;
    }
}
