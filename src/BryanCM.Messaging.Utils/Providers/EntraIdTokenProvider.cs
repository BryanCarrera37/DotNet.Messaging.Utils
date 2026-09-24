using System.Text;
using BryanCM.Messaging.Utils.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace BryanCM.Messaging.Utils.Providers
{
    public class EntraIdTokenProvider : BaseTokenProvider, ITokenProvider
    {
        private readonly EntraIdOptions _settings;
        private readonly ILogger<EntraIdTokenProvider> _logger;
        private readonly IConfidentialClientApplication _confidentialApp;

        private string? _accessToken;
        private DateTimeOffset? _expiresAt;

        public override TimeSpan RetryRefreshTokenMargin => TimeSpan.FromSeconds(60);

        public EntraIdTokenProvider(IOptions<EntraIdOptions> options, ILogger<EntraIdTokenProvider> logger)
        {
            _settings = options.Value;
            _logger = logger;
            _confidentialApp = ConfidentialClientApplicationBuilder.Create(_settings.ClientId)
                .WithClientSecret(Encoding.UTF8.GetString(Convert.FromBase64String(_settings.ClientSecret)))
                .WithAuthority(_settings.Authority)
                .Build();
        }

        public override async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (IsValidToReturnTheToken(_accessToken, _expiresAt))
                return _accessToken!;
            
            _logger.LogInformation("[{Context}] - Acquiring a new JWT using '{ClientId}' client",
                nameof(EntraIdTokenProvider),
                _settings.ClientId);

            var result = await _confidentialApp.AcquireTokenForClient([_settings.Scope])
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            _accessToken = result.AccessToken;
            _expiresAt = result.ExpiresOn;
            return _accessToken;
        }
    }
}
