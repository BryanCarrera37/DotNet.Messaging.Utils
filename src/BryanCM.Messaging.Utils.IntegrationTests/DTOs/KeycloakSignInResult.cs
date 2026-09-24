using System.Text.Json.Serialization;

namespace BryanCM.Messaging.Utils.IntegrationTests.DTOs
{
    internal class KeycloakSignInResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
