using System.Net.Http.Headers;
using BryanCM.Messaging.Utils.Helpers.Interfaces;
using BryanCM.Messaging.Utils.Options;
using BryanCM.Messaging.Utils.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BryanCM.Messaging.Utils.Helpers
{
    public sealed class KeycloakConfigurationHelper : IIdentityConfigurationHelper
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient<ITokenProvider, KeycloakTokenProvider>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<KeycloakOptions>>().Value;
                client.BaseAddress = new Uri($"{settings.Authority}/realms/{settings.Realm}");
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds ?? 60);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }

        public void ConfigureSettings(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KeycloakOptions>(
                configuration.GetSection(KeycloakOptions.SectionName));
        }
    }
}
