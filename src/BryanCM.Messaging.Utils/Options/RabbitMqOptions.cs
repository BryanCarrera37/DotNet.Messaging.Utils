using System.ComponentModel.DataAnnotations;

namespace BryanCM.Messaging.Utils.Options
{
    public class RabbitMqOptions
    {
        public static readonly string SectionName = "MessagingOAuth:RabbitMQ";

        [Required(ErrorMessage = "RabbitMQ.Host is required")]
        public string Host { get; init; } = string.Empty;

        [Required(ErrorMessage = "RabbitMQ.Port is required")]
        public int? Port { get; init; }

        [Required(ErrorMessage = "RabbitMQ.VHost is required")]
        public string VHost { get; init; } = string.Empty;
    }
}
