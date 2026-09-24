using BryanCM.Messaging.Utils.Helpers;
using BryanCM.Messaging.Utils.Helpers.Interfaces;

namespace BryanCM.Messaging.Utils.Factories
{
    public static class IdentityConfigurationHelperFactory
    {
        public static IIdentityConfigurationHelper Create(IdentityProvider identityProvider)
        {
            return identityProvider switch
            {
                IdentityProvider.Keycloak => new KeycloakConfigurationHelper(),
                IdentityProvider.EntraId => new EntraIdConfigurationHelper(),
                _ => throw new ArgumentException($"Unsupported identity provider: {identityProvider}")
            };
        }
    }
}
