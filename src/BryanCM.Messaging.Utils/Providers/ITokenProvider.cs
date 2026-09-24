using System.Text.Json.Serialization;
using System.Text.Json;

namespace BryanCM.Messaging.Utils.Providers
{
    public interface ITokenProvider
    {
        protected static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public TimeSpan RetryRefreshTokenMargin { get; }

        public Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
    }
}
