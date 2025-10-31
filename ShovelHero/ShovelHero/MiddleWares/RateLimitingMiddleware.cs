using System.Collections.Concurrent;
using System.Net;

namespace ShovelHero.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        // 儲存每個 IP 的請求記錄
        private static readonly ConcurrentDictionary<string, RequestInfo> _clients = new();

        // 設定參數
        private readonly int _requestLimit;
        private readonly TimeSpan _timeWindow;

        public RateLimitingMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingMiddleware> logger,
            int requestLimit = 5,  // 預設每分鐘 5 個請求
            int timeWindowMinutes = 1)
        {
            _next = next;
            _logger = logger;
            _requestLimit = requestLimit;
            _timeWindow = TimeSpan.FromMinutes(timeWindowMinutes);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientIp = GetClientIp(context);

            if (string.IsNullOrEmpty(clientIp))
            {
                _logger.LogWarning("無法取得客戶端 IP");
                await _next(context);
                return;
            }

            var requestInfo = _clients.GetOrAdd(clientIp, _ => new RequestInfo());

            object? responseToWrite = null;

            lock (requestInfo)
            {
                var now = DateTime.UtcNow;
                requestInfo.RequestTimes.RemoveAll(time => now - time > _timeWindow);

                if (requestInfo.RequestTimes.Count >= _requestLimit)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                    context.Response.Headers["Retry-After"] = "60";
                    context.Response.ContentType = "application/json";

                    responseToWrite = new
                    {
                        error = "請求過於頻繁",
                        message = $"您已超過每分鐘 {_requestLimit} 次請求的限制，請稍後再試",
                        retryAfter = 60
                    };
                }
                else
                {
                    requestInfo.RequestTimes.Add(now);
                }
            }

            if (responseToWrite != null)
            {
                await context.Response.WriteAsJsonAsync(responseToWrite);
                return;
            }

            // 定期清理過期的 IP 記錄（每 100 個請求清理一次）
            if (DateTime.UtcNow - requestInfo.LastCleanup > TimeSpan.FromMinutes(5))
            {
                CleanupExpiredClients();
                requestInfo.LastCleanup = DateTime.UtcNow;
            }

            await _next(context);
        }

        private string GetClientIp(HttpContext context)
        {
            // 嘗試從 X-Forwarded-For 標頭取得真實 IP（適用於反向代理）
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                return ips[0].Trim();
            }

            // 嘗試從 X-Real-IP 標頭取得
            var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // 使用連線的遠端 IP
            return context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }

        private void CleanupExpiredClients()
        {
            var now = DateTime.UtcNow;
            var expiredKeys = _clients
                .Where(kvp => now - kvp.Value.LastCleanup > TimeSpan.FromMinutes(10))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                _clients.TryRemove(key, out _);
            }

            _logger.LogInformation("清理了 {Count} 個過期的 IP 記錄", expiredKeys.Count);
        }

        private class RequestInfo
        {
            public List<DateTime> RequestTimes { get; set; } = new();
            public DateTime LastCleanup { get; set; } = DateTime.UtcNow;
        }
    }
}
