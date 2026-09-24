using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BryanCM.Messaging.Utils.Helpers.Interfaces
{
    public interface IIdentityConfigurationHelper
    {
        void ConfigureServices(IServiceCollection services);
        void ConfigureSettings(IServiceCollection services, IConfiguration configuration);
    }
}
