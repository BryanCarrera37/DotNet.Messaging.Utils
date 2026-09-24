using System.ComponentModel.DataAnnotations;
using BryanCM.Messaging.Utils.Models;

namespace BryanCM.Messaging.Utils.Options
{
    /// <summary>
    /// Options for the RabbitMQ Resources
    /// </summary>
    public class MessagingOptions
    {
        /// <summary>
        /// Name of the field in the secrets file.
        /// </summary>
        public static readonly string SectionName = "MessagingSettings";

        public string ClientProvidedName { get; init; } = "UNKNOWN";

        [Required(ErrorMessage = "Messaging PayloadEncryptionKey is not configured")]
        public string PayloadEncryptionKey { get; init; } = string.Empty;

        [Required(ErrorMessage = "Messaging PayloadEncryptionKey is not configured")]
        public int RetryCountLimit { get; init; }
        public MessagingResources Resources { get; init; } = default!;
    }

    /// <summary>
    /// Options for the RabbitMQ Resources using a Generic Value for the Queues.
    /// </summary>
    /// <typeparam name="TQueue"></typeparam>
    public class MessagingOptions<TQueue> : MessagingOptions where TQueue : Queue
    {
        public new MessagingResources<TQueue> Resources { get; init; } = default!;
    }
}
