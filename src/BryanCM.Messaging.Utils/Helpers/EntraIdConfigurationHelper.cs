using BryanCM.Messaging.Utils.Helpers.Interfaces;
using BryanCM.Messaging.Utils.Options;
using BryanCM.Messaging.Utils.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BryanCM.Messaging.Utils.Helpers
{
    public class EntraIdConfigurationHelper : IIdentityConfigurationHelper
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenProvider, EntraIdTokenProvider>();
        }

        public void ConfigureSettings(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EntraIdOptions>(
                configuration.GetSection(EntraIdOptions.SectionName));
        }
    }
}
