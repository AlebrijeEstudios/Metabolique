using System.Threading.RateLimiting;

namespace AppVidaSana.RateLimitHelpers
{
    public static class RateLimitHelpers
    {
        public static string GetIpPartitionKey(HttpContext httpContext) =>
        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        public static string GetUserOrIpPartitionKey(HttpContext httpContext) =>
            httpContext.User.Identity?.IsAuthenticated == true
                ? httpContext.User.Identity.Name ?? "unknown"
                : httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        public static RateLimitPartition<string> CreateFixedWindowLimiter(
            string partitionKey, int permitLimit, TimeSpan window) =>
            RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = window
            });
    }
}
