using ShovelHero.Middleware;
using ShovelHero.Middlewares;
using ShovelHero.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 讀取 Rate Limiting 設定
var rateLimitOptions = builder.Configuration
    .GetSection("RateLimit")
    .Get<RateLimitOptions>() ?? new RateLimitOptions();

builder.Services.AddSingleton(rateLimitOptions);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// 註冊資料儲存（暫存在記憶體）
builder.Services.AddSingleton<DataStore>();

var app = builder.Build();

// 安全標頭
app.UseSecurityHeaders();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

// 加入 Rate Limiting 中間件（在路由之前）
if (rateLimitOptions.Enabled)
{
    app.UseMiddleware<RateLimitingMiddleware>(
        rateLimitOptions.RequestLimit,
        rateLimitOptions.TimeWindowMinutes
    );

    app.Logger.LogInformation(
        "Rate Limiting 已啟用：每 {TimeWindow} 分鐘最多 {Limit} 個請求",
        rateLimitOptions.TimeWindowMinutes,
        rateLimitOptions.RequestLimit
    );
}

app.UseAuthorization();

app.MapControllers();

app.Run();