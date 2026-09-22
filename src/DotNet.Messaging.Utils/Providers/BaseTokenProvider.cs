
namespace DotNet.Messaging.Utils.Providers
{
    public abstract class BaseTokenProvider : ITokenProvider
    {
        public abstract TimeSpan RetryRefreshTokenMargin { get; }

        public abstract Task<string> GetTokenAsync(CancellationToken cancellationToken = default);

        protected bool IsValidToReturnTheToken(string? accessToken, DateTimeOffset? expiresAt)
        {
            return !string.IsNullOrEmpty(accessToken)
                && DateTimeOffset.UtcNow + RetryRefreshTokenMargin < expiresAt;
        }
    }
}
