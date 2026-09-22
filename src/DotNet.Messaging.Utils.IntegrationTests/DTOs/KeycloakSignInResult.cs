using System.Text.Json.Serialization;

namespace DotNet.Messaging.Utils.IntegrationTests.DTOs
{
    internal class KeycloakSignInResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
