using Microsoft.AspNetCore.Mvc;

namespace ShovelHero.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 測試 Rate Limiting 的端點
        /// </summary>
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            _logger.LogInformation("Ping request from IP: {IP}", clientIp);

            return Ok(new
            {
                message = "pong",
                timestamp = DateTime.UtcNow,
                clientIp = clientIp
            });
        }

        /// <summary>
        /// 取得目前 Rate Limiting 狀態資訊
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                message = "Rate Limiting is active",
                serverTime = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        }
    }
}
