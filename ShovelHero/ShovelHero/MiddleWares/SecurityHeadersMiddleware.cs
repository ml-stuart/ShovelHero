namespace ShovelHero.Middlewares
{
    /// <summary>
    /// 為應用程式添加安全標頭的中介軟體
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            ILogger<SecurityHeadersMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Content-Security-Policy (CSP)
            // 限制資源載入來源，防止 XSS 攻擊
            context.Response.Headers["Content-Security-Policy"] =
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://unpkg.com https://cdnjs.cloudflare.com; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' data:; " +
                "connect-src 'self' https://localhost:* http://localhost:*; " +
                "frame-ancestors 'none'; " +
                "base-uri 'self'; " +
                "form-action 'self'";

            // X-Frame-Options
            // 防止點擊劫持攻擊 (Clickjacking)
            context.Response.Headers["X-Frame-Options"] = "DENY";

            // X-Content-Type-Options
            // 防止瀏覽器進行 MIME 類型嗅探
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";

            // Strict-Transport-Security (HSTS)
            // 強制使用 HTTPS 連線（僅在 HTTPS 連線時添加）
            if (context.Request.IsHttps)
            {
                context.Response.Headers["Strict-Transport-Security"] =
                    "max-age=31536000; includeSubDomains; preload";
            }

            // X-XSS-Protection (舊版瀏覽器支援)
            // 啟用瀏覽器內建的 XSS 防護
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";

            // Referrer-Policy
            // 控制 Referrer 資訊的發送
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

            // Permissions-Policy (取代舊的 Feature-Policy)
            // 控制瀏覽器功能的使用權限
            context.Response.Headers["Permissions-Policy"] =
                "accelerometer=(), " +
                "camera=(), " +
                "geolocation=(), " +
                "gyroscope=(), " +
                "magnetometer=(), " +
                "microphone=(), " +
                "payment=(), " +
                "usb=()";

            // Cross-Origin-Embedder-Policy (COEP)
            // 防止文件載入未明確授權的跨來源資源
            // 注意：這個標頭可能會影響某些第三方資源的載入，視需求啟用
            // context.Response.Headers["Cross-Origin-Embedder-Policy"] = "require-corp";

            // Cross-Origin-Opener-Policy (COOP)
            // 隔離瀏覽器上下文群組，防止跨來源攻擊
            context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";

            // Cross-Origin-Resource-Policy (CORP)
            // 控制資源是否可被其他來源載入
            context.Response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";

            _logger.LogDebug("已添加安全標頭至回應");

            await _next(context);
        }
    }

    /// <summary>
    /// SecurityHeadersMiddleware 的擴充方法
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}