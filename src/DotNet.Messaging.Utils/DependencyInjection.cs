using DotNet.Messaging.Utils.Factories;
using DotNet.Messaging.Utils.Models;
using DotNet.Messaging.Utils.Options;
using DotNet.Messaging.Utils.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DotNet.Messaging.Utils
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures Options (<see cref="MessagingOptions"/>), a Connection Manager (<see cref="IConnectionManager"/>), and the Messaging OAuth Conf.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> of the Application.</param>
        /// <param name="configuration"><see cref="IConfiguration"/> of the Application.</param>
        /// <param name="identityProvider"><see cref="IdentityProvider"/> to be configured.</param>
        /// <returns><see cref="IServiceCollection"/> with the Messaging and OAuth Options configured.</returns>
        public static IServiceCollection ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration, IdentityProvider identityProvider)
        {
            AddNecessaryConfiguration(services, configuration, identityProvider);
            services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();
            return services;
        }

        /// <summary>
        /// Configures Options (<see cref="MessagingOptions"/>), a Connection Manager (<see cref="IConnectionManager"/>), the Messaging OAuth Conf., and a Custom Publisher.
        /// </summary>
        /// <typeparam name="TPublisher">Custom Publisher that is registered with the <see cref="IMessagePublisher"/> Interface</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/> of the Application.</param>
        /// <param name="configuration"><see cref="IConfiguration"/> of the Application.</param>
        /// <param name="identityProvider"><see cref="IdentityProvider"/> to be configured.</param>
        /// <returns><see cref="IServiceCollection"/> with the Messaging and OAuth Options configured.</returns>
        public static IServiceCollection ConfigureRabbitMQ<TPublisher>(
            this IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider) where TPublisher : class, IMessagePublisher
        {
            RemoveDefaultPublisherIfNecessary(services);
            AddNecessaryConfiguration(services, configuration, identityProvider);
            services.AddScoped<IMessagePublisher, TPublisher>();
            return services;
        }

        /// <summary>
        /// Configures Options (<see cref="MessagingOptions"/>), a Connection Manager (<see cref="IConnectionManager"/>), the Messaging OAuth Conf.,
        /// a Custom Publisher (from <see cref="IMessagePublisher"/>) and a Custom Queue (from <see cref="Queue"/>).
        /// </summary>
        /// <typeparam name="TPublisher">Custom Publisher that is registered with the <see cref="IMessagePublisher"/> Interface</typeparam>
        /// <typeparam name="TQueue">Custom Class for the Queues to be registered.</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/> of the Application.</param>
        /// <param name="configuration"><see cref="IConfiguration"/> of the Application.</param>
        /// <param name="identityProvider"><see cref="IdentityProvider"/> to be configured.</param>
        /// <returns><see cref="IServiceCollection"/> with the Messaging and OAuth Options configured.</returns>
        public static IServiceCollection ConfigureRabbitMQ<TPublisher, TQueue>(
            this IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider) where TPublisher : class, IMessagePublisher where TQueue : Queue
        {
            RemoveDefaultPublisherIfNecessary(services);
            AddNecessaryConfiguration<TQueue>(services, configuration, identityProvider);
            services.AddScoped<IMessagePublisher, TPublisher>();
            return services;
        }

        /// <summary>
        /// Configures Settings from the section "MessagingSettings" with the provided type (<see cref="TOptions"/>), a Connection Manager (<see cref="IConnectionManager"/>), the Messaging OAuth Conf.,
        /// a Custom Publisher (from <see cref="IMessagePublisher"/>) and a Custom Queue (from <see cref="Queue"/>).
        /// </summary>
        /// <typeparam name="TPublisher">Custom Publisher that is registered with the <see cref="IMessagePublisher"/> Interface</typeparam>
        /// <typeparam name="TQueue">Custom Class for the Queues to be registered.</typeparam>
        /// <typeparam name="TOptions">Custom Class for the Options to be registered (Representation of MessagingSettings).</typeparam>
        /// <param name="services"><see cref="IServiceCollection"/> of the Application.</param>
        /// <param name="configuration"><see cref="IConfiguration"/> of the Application.</param>
        /// <param name="identityProvider"><see cref="IdentityProvider"/> to be configured.</param>
        /// <returns><see cref="IServiceCollection"/> with the Messaging and OAuth Options configured.</returns>
        public static IServiceCollection ConfigureRabbitMQ<TPublisher, TQueue, TOptions>(
            this IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider
        ) where TPublisher : class, IMessagePublisher
            where TQueue : Queue
            where TOptions : MessagingOptions<TQueue>
        {
            RemoveDefaultPublisherIfNecessary(services);
            AddNecessaryConfiguration<TQueue, TOptions>(services, configuration, identityProvider);
            services.AddScoped<IMessagePublisher, TPublisher>();
            return services;
        }

        private static void AddNecessaryConfiguration(
            IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider)
        {
            services.AddOptions<MessagingOptions>()
                .Bind(configuration.GetSection(MessagingOptions.SectionName))
                .ValidateOnStart();

            AddCommonConfigurations(services, configuration, identityProvider);
        }

        private static void AddNecessaryConfiguration<TQueue>(
            IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider) where TQueue : Queue
        {
            services.AddOptions<MessagingOptions<TQueue>>()
                .Bind(configuration.GetSection(MessagingOptions.SectionName))
                .ValidateOnStart();

            AddCommonConfigurations(services, configuration, identityProvider);
        }

        private static void AddNecessaryConfiguration<TQueue, TOptions>(
            IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider) where TQueue : Queue where TOptions : MessagingOptions<TQueue>
        {
            services.AddOptions<TOptions>()
                .Bind(configuration.GetSection(MessagingOptions.SectionName))
                .ValidateOnStart();

            AddCommonConfigurations(services, configuration, identityProvider);
        }

        private static void AddCommonConfigurations(
            IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider)
        {
            services.ConfigureMessagingOAuth(configuration, identityProvider);
            services.AddSingleton<IConnectionManager, RabbitMqConnectionManager>();
        }

        private static IServiceCollection ConfigureMessagingOAuth(
            this IServiceCollection services,
            IConfiguration configuration,
            IdentityProvider identityProvider)
        {
            services.Configure<RabbitMqOptions>(
                configuration.GetSection(RabbitMqOptions.SectionName));

            services.AddSingleton<IConnectionFactorySsoSecured, RabbitMqConnectionFactory>();
            services.AddSingleton(sp =>
            {
                var rabbitmqSettings = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                return new ConnectionFactory
                {
                    HostName = rabbitmqSettings.Host,
                    Port = (int)rabbitmqSettings.Port!,
                    VirtualHost = rabbitmqSettings.VHost,
                    AutomaticRecoveryEnabled = true,
                    TopologyRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
                };
            });

            var identityConfigurationHelper = IdentityConfigurationHelperFactory.Create(identityProvider);
            identityConfigurationHelper.ConfigureSettings(services, configuration);
            identityConfigurationHelper.ConfigureServices(services);
            return services;
        }

        private static void RemoveDefaultPublisherIfNecessary(IServiceCollection services)
        {
            var publisher = services.FirstOrDefault(descriptor => descriptor.ServiceType.Name == typeof(RabbitMqMessagePublisher).Name);
            if (publisher is not null)
                services.Remove(publisher);
        }
    }
}
