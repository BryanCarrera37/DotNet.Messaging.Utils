using System.ComponentModel.DataAnnotations;

namespace DotNet.Messaging.Utils.Options
{
    public sealed class KeycloakOptions
    {
        public static readonly string SectionName = "MessagingOAuth:Keycloak";

        [Required(ErrorMessage = "Keycloak.GrantType is required")]
        public string GrantType { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.Scope is required")]
        public string Scope { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.Authority is required")]
        public string Authority { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.Realm is required")]
        public string Realm { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.ClientId is required")]
        public string ClientId { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.ClientSecret is required")]
        public string ClientSecret { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.Username is required")]
        public string Username { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.Password is required")]
        public string Password { get; init; } = string.Empty;

        [Required(ErrorMessage = "Keycloak.TimeoutSeconds")]
        public int? TimeoutSeconds { get; init; }
    }
}
