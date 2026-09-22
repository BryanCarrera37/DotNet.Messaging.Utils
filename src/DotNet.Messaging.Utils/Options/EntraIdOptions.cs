using System.ComponentModel.DataAnnotations;

namespace DotNet.Messaging.Utils.Options
{
    public sealed class EntraIdOptions
    {
        public static readonly string SectionName = "MessagingOAuth:EntraId";

        [Required(ErrorMessage = "EntraId.ClientId is required")]
        public string ClientId { get; init; } = string.Empty;

        [Required(ErrorMessage = "EntraId.ClientSecret is required")]
        public string ClientSecret { get; init; } = string.Empty;

        [Required(ErrorMessage = "EntraId.Scope is required")]
        public string Scope { get; init; } = string.Empty;

        [Required(ErrorMessage = "EntraId.Authority is required")]
        public string Authority { get; init; } = string.Empty;
    }
}
