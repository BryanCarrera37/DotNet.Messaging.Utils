using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DotNet.Messaging.Utils.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotNet.Messaging.Utils.Providers
{
    internal class TokenSignInResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; init; }
        [JsonPropertyName("token_type")] public string TokenType { get; init; } = string.Empty;
        [JsonPropertyName("scope")] public string Scope { get; init; } = string.Empty;
    }

    public class KeycloakTokenProvider(
        HttpClient httpClient,
        IOptions<KeycloakOptions> options,
        ILogger<KeycloakTokenProvider> logger) : ITokenProvider
    {
        private readonly static string _context = "KeycloakTokenProvider";
        private readonly static int _refreshTokenMarginSeconds = 60;

        private readonly HttpClient _httpclient = httpClient;
        private readonly KeycloakOptions _keycloakSettings = options.Value;
        private readonly ILogger<KeycloakTokenProvider> _logger = logger;
        private readonly string _signInUrl = $"/realms/{options.Value.Realm}/protocol/openid-connect/token";

        private string? _accessToken;
        private DateTimeOffset? _expiresAt;

        public TimeSpan RetryRefreshTokenMargin => TimeSpan.FromSeconds(_refreshTokenMarginSeconds);

        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (IsValidToReturnTheToken())
            {
                return _accessToken!;
            }

            using var resp = await _httpclient.PostAsync(_signInUrl, GetSignInData(), cancellationToken)
                .ConfigureAwait(false);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("[{Context}] - Cannot get JWT using Client ID: '{ClientId}' => {Response}",
                    _context,
                    _keycloakSettings.ClientId,
                    await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
                throw new HttpIOException(
                    HttpRequestError.UserAuthenticationError,
                    $"Could not Sign In correctly using Client ID: '{_keycloakSettings.ClientId}'");
            }

            var responseBody = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var data = JsonSerializer.Deserialize<TokenSignInResponse>(responseBody, ITokenProvider.Options);
            if (string.IsNullOrEmpty(data?.AccessToken))
            {
                _logger.LogError("[{Context}] - JWT wasn't found in the response => '{Response}'", _context, responseBody);
                throw new HttpIOException(
                    HttpRequestError.InvalidResponse,
                    "Cannot get the JWT from the response, possible malformed response");
            }

            _accessToken = data.AccessToken;
            _expiresAt = DateTimeOffset.UtcNow.AddSeconds(data.ExpiresIn);
            return _accessToken;
        }

        private bool IsValidToReturnTheToken()
        {
            return !string.IsNullOrEmpty(_accessToken)
                && DateTimeOffset.UtcNow + RetryRefreshTokenMargin < _expiresAt;
        }

        private FormUrlEncodedContent GetSignInData() =>
            new(new Dictionary<string, string>
            {
                { "grant_type", _keycloakSettings.GrantType },
                { "scope", _keycloakSettings.Scope },
                { "client_id", _keycloakSettings.ClientId },
                { "client_secret", Encoding.UTF8.GetString(Convert.FromBase64String(_keycloakSettings.ClientSecret)) },
                { "username", _keycloakSettings.Username },
                { "password", Encoding.UTF8.GetString(Convert.FromBase64String(_keycloakSettings.Password)) },
            });
    }
}
